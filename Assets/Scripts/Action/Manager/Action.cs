using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public interface IActionCompleted
	{
		void OnActionCompleted (Action action);
	}


	public class Action : MonoBehaviour
	{
		public void Free ()
		{
			DestroyImmediate (this);	// 卸载脚本
		}
	}


	public class ActionSingle : Action
	{
		// 独立动作
	}


	public class ActionThread : Action
	{
		// 同存动作
	}


	public class ActionName
	{
		// 判断动作是否合法，返回动作类型
		public static ActionType IsValid (string actionName)
		{
			string[] sitActionNames = System.Enum.GetNames (typeof(SitActionName));
			for (int i = 0; i < sitActionNames.Length; ++i) {
				if (actionName == sitActionNames [i])
					return ActionType.sit;
			}
			string[] standActionNames = System.Enum.GetNames (typeof(StandActionName));
			for (int i = 0; i < standActionNames.Length; ++i) {
				if (actionName == standActionNames [i])
					return ActionType.stand;
			}
			return ActionType.error;
		}


		// 返回含有起始和结束动作的根动作名称（不包含begin/end），如果没有则返回null
		public static string FindBorder (string actionName)
		{
			string[] sitActionNames = System.Enum.GetNames (typeof(SitActionWithBorder));
			for (int i = 0; i < sitActionNames.Length; ++i) {
				if (actionName == sitActionNames [i]) {
					return GetSitRoot (actionName);
				}
			}
			string[] standActionNames = System.Enum.GetNames (typeof(StandActionWithBorder));
			for (int i = 0; i < standActionNames.Length; ++i) {
				if (actionName == standActionNames [i]) {
					return GetStandRoot (actionName);
				}
			}
			return null;
		}


		// 特殊的，处理具有相同根动作的动作，需确保输入的动作是有起始和结束动作的
		public static string GetSitRoot (string actionName)
		{
			if (actionName == SitActionWithBorder.sit_cross_arm_shake_head.ToString ()) {
				return SitActionWithBorder.sit_cross_arm.ToString ();
			}
			if (actionName == SitActionWithBorder.sit_crumple_paper.ToString ()) {
				return SitActionWithBorder.sit_draw_pen.ToString ();
			}
			return actionName;
		}


		// 特殊的，处理具有相同根动作的动作，需确保输入的动作是有起始和结束动作的
		public static string GetStandRoot (string actionName)
		{
			if (actionName == StandActionWithBorder.stand_cross_arm_shake_head.ToString ())
				return StandActionWithBorder.stand_cross_arm.ToString ();
			if (actionName == StandActionWithBorder.other_exercise_3.ToString ())
				return StandActionWithBorder.other_exercise_2.ToString ();
			return actionName;
		}


		// 返回动作附带的物品路径
		public static string[] FindItems (string stateName)
		{
			switch (stateName) {
			// normal display (object under such path simply set shown or hidden when animation begin or end)
			case "stand_talk_mic":
				return new string[] {
					"mic"
				};
			case "stand_talk_paper":
				return new string[] {
					"Polygon"
				};
			case "stand_talk_megaphone":
				return new string[] {
					"speaker"
				};
			case "stand_talk_megaphone_point_far_times":
				return new string[] {
					"speaker"
				};
			case "stand_talk_interphone":
				return new string[] {
					"interphone"
				};
			case "stand_talk_interphone_point_far_times":
				return new string[] {
					"interphone"
				};
			case "stand_play_cellphone_pickup":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_hand_Goal/phone"
				};
			case "stand_play_cellphone_putdown":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_hand_Goal/phone"
				};
			case "stand_write_pen":
				return new string[] {
					"pen",
					"questionnaire"
				};
			case "stand_camera_pickup":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/camera2"
				};
			case "stand_camera_putdown":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/camera2"
				};
			case "stand_tailor_tape_model":
				return new string[] {
					"tailor"
				};
			case "stand_controller":
				return new string[] {
					"caculator"
				};
			case "stand_drink_water":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_hand_Goal/watercup"
				};
			case "stand_insert":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_hand_Goal/insertcube",
					"hip_ctrl/root/spline/right_chest/right_hand_Goal/insertstick"
				};
			case "stand_light":
				return new string[] {
					"spotlight"
				};
			case "stand_mic_self":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/left_thumb/mic"
				};
			case "stand_photo_cellphone":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/phone"
				};
			case "stand_pour_wine":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/left_thumb/left_thumbmid/wine"
				};
			case "stand_record_rod":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/right_arm/right_elbow/right_hand/right_mid/right_mid2/mic"
				};
			case "stand_recorder_pickup":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/left_mid/left_mid2/pen"
				};
			case "stand_recorder_putdown":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/left_mid/left_mid2/pen"
				};
			case "stand_tailor_tape":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/right_hand_Goal/tailor"
				};
			case "stand_talk_cellphone_pickup":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/phone"
				};
			case "stand_talk_cellphone_putdown":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/phone"
				};
			case "stand_water_flower":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/left_mid/left_mid2/waterpot"
				};


			case "sit_play_cellphone_pickup":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_hand_Goal/phone"
				};
			case "sit_play_cellphone_putdown":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_hand_Goal/phone"
				};
			case "sit_talk_cellphone_pickup":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_hand_Goal/phone"
				};
			case "sit_talk_cellphone_putdown":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_hand_Goal/phone"
				};
			case "sit_ipad_pickup":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_hand_Goal/phone"
				};
			case "sit_ipad_putdown":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_hand_Goal/phone"
				};
			case "sit_write_dictate":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_hand_Goal/pen",
					"hip_ctrl/root/spline/right_chest/right_hand_Goal/book"
				};
			case "sit_draw_pen":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_hand_Goal/pen",
					"paper2_1"
				};
			case "sit_crumple_paper":
				return new string[] {
					"paper2_1"
				};

			// create in scene (object under such path should be hidden, but used as a target to be followed by object in scene)
			case "stand_poster":
				return new string[] {
					"poster"
				};
			case "stand_stickynote_wall":
				return new string[] {
					"stickynote"
				};
			
			// gain from scene (object under such path should be hidden, but used as a target to be followed by object in scene)
			case "stand_fixphone_pickup":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/mic_2"
				};
			case "stand_fixphone_putdown":
				return new string[] {
					"hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/mic_2"
				};
			case "other_exercise_1":
				return new string[] {
					"barbell"
				};
			case "other_exercise_2":
				return new string[] {
					"dumbbell1",
					"dumbbell2"
				};
			case "other_exercise_3":
				return new string[] {
					"dumbbell1",
					"dumbbell2"
				};
			case "stand_wine_pickup":
				return new string[] {
					"cup"
				};
			case "stand_wine_putdown":
				return new string[] {
					"cup"
				};
			default:
				return null;
			}
		}

		// 返回动作需要创建的物品名称
		public static string FindPrefabs (string stateName)
		{
			switch (stateName) {
			case "stand_poster":
				return "poster";
			case "stand_stickynote_wall":
				return "stickynote";
			default:
				return null;
			}
		}

		public static StuffType FindStuffType (string stateName)
		{
			switch (stateName) {
			case "stand_small_pickup_table":
				return StuffType.SmallStuff;
			case "stand_middle_pickup_table":
				return StuffType.MiddleStuff;
			case "stand_large_pickup_table":
				return StuffType.BigStuff;
			case "stand_book_pickup_table":
				return StuffType.BookStuff;
			default:
				return StuffType.SmallStuff;
			}
		}
	}

	public enum ActionType
	{
		error,
		sit,
		stand
	}

	public enum SitActionName
	{
		sit,
		stand_up,
		sit_head_left30,
		sit_head_left90,
		sit_head_right30,
		sit_head_right90,
		sit_breathe,
		sit_shake_leg,
		//sit_sprawl,
		sit_look_up,
		sit_relax_1,
		//sit_relax_2,
		sit_nod,
		sit_shake_head,
		sit_cross_arm,
		sit_cross_arm_shake_head,
		sit_scratch_head,
		sit_chin_in_hand,
		sit_scratch_head_computer,
		sit_chin_in_hand_computer,
		sit_sleep,
		sit_raise_right_hand,
		sit_hello,
		sit_watch,
		sit_applaud,
		sit_laugh_loud,
		sit_backward,
		sit_hold_head_back,
		//sit_swing_with_music,
		sit_computer,
		sit_keyboard,
		sit_mouse_move,
		//sit_fixphone,
		//sit_fixphone_pickup,
		//sit_fixphone_putdown,
		//sit_fixphone_dial,
		//sit_hold_cellphone,
		//sit_play_cellphone_scroll,
		//sit_play_cellphone_click,
		//sit_play_cellphone_pickup,
		//sit_play_cellphone_putdown,
		sit_photo_cellphone,
		//sit_talk_cellphone,
		//sit_talk_cellphone_pickup,
		//sit_talk_cellphone_putdown,
		//sit_listen_cellphone,
		//sit_ipad,
		//sit_ipad_pickup,
		//sit_ipad_putdown,
		//sit_ipad_click,
		//sit_ipad_scroll,
		sit_write_dictate,
		sit_draw_pen,
		sit_crumple_paper,
		sit_left30,
		sit_right30,
		sit_knock_table,
		sit_talk,
		sit_talk_point_near,
		sit_talk_point_far,
		sit_talk_look_up
	}

	public enum StandActionName
	{
		stand,
		sit_down,
		walk_blend_tree,
		walk_small_blend_tree,
		walk_middle_blend_tree,
		walk_book_blend_tree,
		stand_head_left30,
		stand_head_left90,
		stand_head_right30,
		stand_head_right90,
		stand_left30,
		stand_shake_head,
		stand_cross_arm,
		stand_chin_in_hand,
		stand_raise_hand,
		stand_hello,
		stand_watch,
		stand_applaud,
		stand_talk,
		stand_talk_point_ppt,
		stand_talk_point_near,
		stand_talk_point_far,
		stand_talk_point_near_times,
		stand_talk_mic,
		stand_talk_paper,
		stand_talk_megaphone,
		stand_talk_megaphone_point_far_times,
		stand_talk_interphone,
		stand_talk_interphone_point_far_times,
		stand_laugh_loud,
		stand_bow,
		stand_fixphone,
		stand_fixphone_pickup,
		stand_fixphone_putdown,
		stand_fixphone_dial,
		stand_hold_cellphone,
		stand_play_cellphone_scroll,
		stand_play_cellphone_click,
		stand_play_cellphone_pickup,
		stand_play_cellphone_putdown,
		stand_write_pen,
		stand_small_hold,
		stand_small_pickup_table,
		stand_small_putdown_table,
		stand_small_pickup_bag,
		stand_small_putdown_bag,
		//stand_small_pickup_ground,
		inter_small_give,
		inter_small_take,
		stand_middle_hold,
		stand_middle_pickup_table,
		stand_middle_putdown_table,
		stand_middle_pickup_bag,
		stand_middle_putdown_bag,
		inter_middle_give,
		inter_middle_take,
		stand_large_hold_front,
		stand_large_pickup_table_front,
		stand_large_putdown_table_front,
		//inter_paper_give,
		//inter_paper_take,
		stand_book_hold,
		stand_book_pickup_table,
		stand_book_putdown_table,
		stand_book_pickup_bag,
		stand_book_putdown_bag,
		inter_book_give,
		inter_book_take,
		stand_camera,
		stand_camera_pickup,
		stand_camera_putdown,
		stand_tailor_tape_model,
		stand_poster,
		stand_stickynote_wall,
		stand_write_wall,
		other_exercise_1,
		other_exercise_2,
		other_exercise_3,
		stand_breathe,
		stand_controller,
		stand_drink_water,
		stand_insert,
		stand_jump_cheer,
		stand_knock_table,
		stand_light,
		stand_look_up,
		stand_mic_self,
		stand_mirror_clothes,
		stand_nod,
		stand_photo_cellphone,
		stand_pour_wine,
		stand_record_rod,
		stand_recorder,
		stand_recorder_pickup,
		stand_recorder_putdown,
		stand_tailor_tape,
		stand_talk_cellphone,
		stand_talk_cellphone_listen,
		stand_talk_cellphone_pickup,
		stand_talk_cellphone_putdown,
		stand_talk_look_down,
		stand_talk_point_far_times,
		stand_water_flower,
		stand_wine,
		stand_wine_toast,
		stand_wine_pickup,
		stand_wine_putdown,
		drag_float
	}

	public enum SitActionWithBorder
	{
		sit_head_left30,
		sit_head_left90,
		sit_head_right30,
		sit_head_right90,
		sit_breathe,
		sit_shake_leg,
		sit_sprawl,
		sit_look_up,
		sit_relax_2,
		sit_cross_arm,
		sit_cross_arm_shake_head,
		sit_sleep,
		sit_raise_right_hand,
		sit_applaud,
		sit_backward,
		sit_hold_head_back,
		sit_swing_with_music,
		sit_write_dictate,
		sit_draw_pen,
		sit_crumple_paper,
		sit_left30,
		sit_right30,
		sit_talk_look_up
	}

	public enum StandActionWithBorder
	{
		stand_head_left30,
		stand_head_left90,
		stand_head_right30,
		stand_head_right90,
		stand_left30,
		stand_cross_arm,
		stand_cross_arm_shake_head,
		stand_chin_in_hand,
		stand_raise_hand,
		stand_applaud,
		stand_write_pen,
		other_exercise_2,
		other_exercise_3,
		stand_controller,
		stand_knock_table,
		stand_light,
		stand_look_up,
		stand_mic_self,
		stand_mirror_clothes,
		stand_record_rod,
		stand_talk_look_down,
		drag_float
	}
}