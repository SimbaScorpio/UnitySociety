using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	[System.Serializable]
	public class Storyline
	{
		public CharacterData[] characterdata;
		public InitCharacterData[] initcharacterdata;
		public CompositeMovementData[] compositemovementdata;
		public SceneData[] scenedata;
		public RandomActivity[] random;
	}
}