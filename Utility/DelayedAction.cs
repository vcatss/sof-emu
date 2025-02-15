﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace Utility
{
    public class DelayedAction
    {
        protected static List<DelayedAction> ActionList = new List<DelayedAction>();

        public static void CheckActions()
        {
            for (int i = 0; i < ActionList.Count; i++)
            {
                try
                {
                    ActionList[i].Check();
                }
                catch
                {
                    //Collection modified
                }

                if ((i & 511) == 0)
                    Thread.Sleep(1);
            }
        }

        protected Action Action;

        protected long InvokeUtc;

        public DelayedAction(Action action, int delay)
        {
            Action = action;
            InvokeUtc = Funcs.GetCurrentMilliseconds() + delay;

            ActionList.Add(this);
        }

        public void Abort()
        {
            ActionList.Remove(this);
        }

        protected void Check()
        {
            if (Funcs.GetCurrentMilliseconds() < InvokeUtc)
                return;

            try
            {
                Action.Invoke();
            }
            catch (Exception ex)
            {
                Log.ErrorException("DelayedAction Ex:", ex);
            }
            finally
            {
                Abort();
            }
        }
    }
}
