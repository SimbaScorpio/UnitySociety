using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class NetworkPlayerResources : NetworkBehaviour
	{
		[SyncVar (hook = "OnPlayerCharacterChanged")]
		public CharacterData playerCharacterData;

		private CharacterGenerator generator;
		private GameObject ghost;

		#region instance

		private static NetworkPlayerResources instance;

		public static NetworkPlayerResources GetInstance ()
		{
			return instance;
		}

		#endregion

		void Start ()
		{
			generator = CharacterGenerator.GetInstance ();
			if (playerCharacterData != null) {
				generator.Generate (gameObject, playerCharacterData);
			}
			if (isLocalPlayer) {
				instance = this;
				playerCharacterData = new CharacterData ();
				playerCharacterData.body_type = "男瘦";
				playerCharacterData.clothing = "1_衬衣短裤1";
				playerCharacterData.hair = 1;
				playerCharacterData.bag = 0;
				playerCharacterData.glasses = 0;
				generator.Generate (gameObject, playerCharacterData);
				CmdChangeCharacter (playerCharacterData);
				ghost = GetComponent<NetworkPlayerTeleport> ().ghost;
				if (ghost != null)
					generator.Generate (ghost, playerCharacterData);
				FileManager.GetInstance ().LoadKeywordData ();
			}
		}

		public void RandomlyChangeClothes ()
		{
			if (isLocalPlayer) {
				Dictionary<string, Texture2D> clothes = MaterialCollection.GetInstance ().clothesTextures;
				int index = Random.Range (0, clothes.Count);
				int count = 0;
				foreach (string name in clothes.Keys) {
					if (count++ == index) {
						playerCharacterData.clothing = name;
						generator.Generate (gameObject, playerCharacterData);
						if (ghost != null)
							generator.Generate (ghost, playerCharacterData);
						CmdChangeCharacter (playerCharacterData);
						break;
					}
				}
			}
		}

		public void RandomlyChangeBodyType ()
		{
			if (isLocalPlayer) {
				if (playerCharacterData.body_type == "男瘦") {
					playerCharacterData.body_type = "男胖";
				} else if (playerCharacterData.body_type == "男胖") {
					playerCharacterData.body_type = "女裙";
					playerCharacterData.hair = 2;
				} else if (playerCharacterData.body_type == "女裙") {
					playerCharacterData.body_type = "女裤";
				} else if (playerCharacterData.body_type == "女裤") {
					playerCharacterData.body_type = "男瘦";
					playerCharacterData.hair = 1;
				}
				generator.Generate (gameObject, playerCharacterData);
				if (ghost != null)
					generator.Generate (ghost, playerCharacterData);
				CmdChangeCharacter (playerCharacterData);
			}
		}

		[Command]
		void CmdChangeCharacter (CharacterData cdata)
		{
			OnPlayerCharacterChanged (cdata);
		}

		void OnPlayerCharacterChanged (CharacterData playerCharacterData)
		{
			if (!isLocalPlayer) {
				this.playerCharacterData = playerCharacterData;
				generator.Generate (gameObject, playerCharacterData);
			}
		}
	}
}