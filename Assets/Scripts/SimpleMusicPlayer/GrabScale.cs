//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GrabScale : MonoBehaviour {


//    public OVRGrabber left_hand;
//    public OVRGrabber right_hand;

//    Vector3 left_hand_begin_pos;
//    Vector3 right_hand_begin_pos;
//    Vector3 start_scale;

//    bool start_point_record;

//    GameObject scale_object;

//    private void FixedUpdate()
//    {
//        if (left_hand.on_grab == false || right_hand.on_grab == false) return;

//        if (left_hand.grabbedObject != null || right_hand.grabbedObject != null)
//        {
//            scale_object = left_hand.grabbedObject != null ? left_hand.grabbedObject.gameObject  : right_hand.grabbedObject.gameObject;

//            if (left_hand.on_grab && right_hand.on_grab)
//            {
//                if (!start_point_record)
//                {
//                    start_point_record = true;

//                    left_hand_begin_pos = left_hand.transform.position;
//                    right_hand_begin_pos = right_hand.transform.position;

//                    start_scale = scale_object.transform.localScale;

//                }
//                else
//                {
//                    float distance_now = Vector3.Distance(left_hand.transform.position, right_hand.transform.position);
//                    float distance_begin = Vector3.Distance(left_hand_begin_pos, right_hand_begin_pos);
//                    float scale = distance_now / distance_begin;
//                    scale_object.transform.localScale = start_scale * scale;
//                }
//            }
//            else
//            {
//                start_point_record = false;
//            }
           
//        }
//        else
//        {
//            start_point_record = false;
//        }
//    }


//}
