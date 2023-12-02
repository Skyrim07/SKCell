using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    /// <summary>
    /// Manages SKColliderResponder instances. 
    /// </summary>
    public class SKCldResponderManager : SKSingleton<SKCldResponderManager>
    {
        public static Dictionary<string, SKColliderResponder> responderDict = new Dictionary<string, SKColliderResponder>();

        public static SKColliderResponder lastResponder;

        /// <summary>
        /// Get a responder by its uid.
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static SKColliderResponder GetResponder(string uid)
        {
            return SKUtils.GetValueInDictionary(responderDict, uid);
        }

        /// <summary>
        /// Get the last responder which invokes a valid event.
        /// </summary>
        /// <returns></returns>
        public static SKColliderResponder GetLastResponder()
        {
            return lastResponder;
        }

        public static void AddResponder(SKColliderResponder responder)
        {
            SKUtils.InsertOrUpdateKeyValueInDictionary(responderDict, responder.uid, responder);
        }

        public static void UpdateResponderUID(SKColliderResponder responder, string formerUID, string newUID)
        {
            SKUtils.RemoveKeyInDictionary(responderDict, formerUID);
            responder.uid = newUID;
            AddResponder(responder);
        }
    }
}
