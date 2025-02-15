﻿/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Reflection;
using UnityEditor;
using Facebook.WitAi.Data.Configuration;

namespace Facebook.WitAi.Windows
{
    public static class WitWindowUtility
    {
        // Window types
        public static Type SetupWindowType => FindChildClass(typeof(WitWelcomeWizard));
        public static Type ConfigurationWindowType => FindChildClass(typeof(WitWindow));
        public static Type UnderstandingWindowType => FindChildClass(typeof(WitUnderstandingViewer));
        // Finds a child class if possible
        private static Type FindChildClass(Type baseType)
        {
            Type result = baseType;
            Assembly currentAssembly = baseType.Assembly;
            Array.Find(AppDomain.CurrentDomain.GetAssemblies(), (assembly) =>
            {
                if (assembly != currentAssembly)
                {
                    Type[] types = assembly.GetTypes();
                    int index = Array.FindIndex(types, (assemblyType) => { return assemblyType.BaseType == baseType; });
                    if (index != -1)
                    {
                        result = types[index];
                        return true;
                    }
                }
                return false;
            });
            return result;
        }

        // Opens Setup Window
        public static void OpenSetupWindow(Action<WitConfiguration> onSetupComplete)
        {
            // Get wizard (Title is overwritten)
            WitWelcomeWizard wizard = (WitWelcomeWizard)ScriptableWizard.DisplayWizard(WitTexts.Texts.SetupTitleLabel, SetupWindowType, WitTexts.Texts.SetupSubmitButtonLabel);
            // Set success callback
            wizard.successAction = onSetupComplete;
        }
        // Opens Configuration Window
        public static void OpenConfigurationWindow(WitConfiguration configuration = null)
        {
            // Setup if needed
            if (configuration == null && !WitConfigurationUtility.HasValidCustomConfig())
            {
                OpenSetupWindow(OpenConfigurationWindow);
                return;
            }

            // Get window & show
            WitConfigurationWindow window = (WitConfigurationWindow)EditorWindow.GetWindow(ConfigurationWindowType);
            window.autoRepaintOnSceneChange = true;
            window.SetConfiguration(configuration);
            window.Show();
        }
        // Opens Understanding Window to specific configuration
        public static void OpenUnderstandingWindow(WitConfiguration configuration = null)
        {
            // Setup if needed
            if (configuration == null && !WitConfigurationUtility.HasValidCustomConfig())
            {
                OpenSetupWindow(OpenUnderstandingWindow);
                return;
            }

            // Get window & show
            WitConfigurationWindow window = (WitConfigurationWindow)EditorWindow.GetWindow(UnderstandingWindowType);
            window.autoRepaintOnSceneChange = true;
            window.SetConfiguration(configuration);
            window.Show();
        }
    }
}
