using System.Collections.Generic;


namespace SKCell
{
    /// <summary>
    /// Dispatches event. All in-game events should be accessed here.
    /// </summary>
    public sealed class EventDispatcher : SKSingleton<EventDispatcher>
    {
        //Event Handler Identifiers
        public const int EH_COMMON = 0;
        public const int EH_PLAYER = 1;
        public const int EH_ITEM = 2;
        public const int EH_COMBAT = 3;
        public const int EH_UI = 4;
        public const int EH_ENV = 5;
        public const int EH_FX = 6;

        public static Dictionary<int, EventHandler> handlerDict = new Dictionary<int, EventHandler>();
        public static EventHandler Common { get { return m_GetHandler(EH_COMMON); } }
        public static EventHandler Player { get { return m_GetHandler(EH_PLAYER); } }
        public static EventHandler Item { get { return m_GetHandler(EH_ITEM); } }
        public static EventHandler Combat { get { return m_GetHandler(EH_COMBAT); } }
        public static EventHandler UI { get { return m_GetHandler(EH_UI); } }
        public static EventHandler Env { get { return m_GetHandler(EH_ENV); } }
        public static EventHandler FX { get { return m_GetHandler(EH_FX); } }

        /// <summary>
        /// Dispatch an event in an EventHandler with int id. Find id in EventRef class.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="id"></param>
        public static void Dispatch(EventHandler handler, int id)
        {
            handler.DispatchEvent(id);
        }

        /// <summary>
        /// Add an event listener in an EventHandler with int id. Find id in EventRef class.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="id"></param>
        public static bool AddListener(EventHandler handler, int id, SKEvent t_event)
        {
            if (handler == null)
            {
                SKUtils.EditorLogWarning("EventDispatcher.AddListener() --- event handler is null.");
                return false;
            }
            handler.AddListener(id, t_event);
            return true;
        }

        /// <summary>
        /// Remove an event listener in an EventHandler with int id.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="id"></param>
        public static bool RemoveListener(EventHandler handler, int id, SKEvent t_event)
        {
            if (handler == null)
            {
                SKUtils.EditorLogWarning("EventDispatcher.RemoveListener() --- event handler is null.");
                return false;
            }
            handler.RemoveListener(id, t_event);
            return true;
        }

        /// <summary>
        /// Register an EventHandler in the handler dict.
        /// </summary>
        public static bool RegisterHandler(EventHandler handler, int id)
        {
            return SKUtils.InsertOrUpdateKeyValueInDictionary(handlerDict, id, handler);
        }

        private static EventHandler m_GetHandler(int type)
        {
            if (!handlerDict.ContainsKey(type))
            {
                RegisterHandler(new EventHandler(type), type);
            }

            return SKUtils.GetValueInDictionary(handlerDict, type);
        }
    }
}
