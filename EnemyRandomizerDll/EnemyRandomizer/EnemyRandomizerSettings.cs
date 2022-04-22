using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
using ModCommon;

namespace EnemyRandomizerMod
{
    //string lookup names for the config options
    public class EnemyRandomizerSettingsVars
    {
        public const string Seed = "Seed";
        public const string CustomSeed = "CustomSeed";

        public const string RNGChaosMode = "RNGChaosMode";
        public const string RNGRoomMode = "RNGRoomMode";
        public const string RandomizeGeo = "RandomizeGeo";
        public const string CheatNoclip = "(Cheat) No Clip";
        public const string CustomEnemies = "CustomEnemies";
        public const string GodmasterEnemies = "GodmasterEnemies";

        //change when the global settings are updated to force a recreation of the global settings
        public const string GlobalSettingsVersion = "0.0.9";
    }

    //Global (non-player specific) settings
    public class EnemyRandomizerSettings
    {
        public Dictionary<string, bool> BoolValues = new();
        public Dictionary<string, string> StringValues = new();
        public bool GetBool(bool b,string s)
        {
            if(BoolValues.TryGetValue(s, out bool flag))return flag;
            return b;
        }
        public void SetBool(bool b,string s)=>BoolValues[s] = b;
        public void Reset()
        {
            BoolValues.Clear();
            StringValues.Clear();

            //foreach(string s in EnemyRandomizerDatabase.enemyTypeNames )
            //{
            //    StringValues.Add( s, s );
            //    BoolValues.Add( s, true );
            //}
        }


        public string SettingsVersion = "0.0.0";

        public bool RNGChaosMode {
            get => GetBool( false ,EnemyRandomizerSettingsVars.RNGChaosMode);
            set {
                StringValues[EnemyRandomizerSettingsVars.RNGChaosMode] = "Chaos Mode";
                SetBool( value , EnemyRandomizerSettingsVars.RNGChaosMode);
            }
        }
        public bool NoClip
        {
            get => GetBool(false, EnemyRandomizerSettingsVars.CheatNoclip);
            set
            {
                StringValues[EnemyRandomizerSettingsVars.CheatNoclip] = "(Cheat) No Clip";
                SetBool(value, EnemyRandomizerSettingsVars.CheatNoclip);
            }
        }
        public bool RNGRoomMode {
            get => GetBool( false,EnemyRandomizerSettingsVars.RNGRoomMode );
            set {
                StringValues[EnemyRandomizerSettingsVars.RNGRoomMode] = "Room Mode";
                SetBool( value,EnemyRandomizerSettingsVars.RNGRoomMode );
            }
        }

        public bool RandomizeGeo {
            get => GetBool( false,EnemyRandomizerSettingsVars.RandomizeGeo );
            set {
                StringValues[ EnemyRandomizerSettingsVars.RandomizeGeo ] = "Randomize Geo";
                SetBool( value ,EnemyRandomizerSettingsVars.RandomizeGeo);
            }
        }

        public bool CustomEnemies {
            get => GetBool( false,EnemyRandomizerSettingsVars.CustomEnemies );
            set {
                StringValues[ EnemyRandomizerSettingsVars.CustomEnemies ] = "Custom Enemies";
                SetBool( value,EnemyRandomizerSettingsVars.CustomEnemies );
            }
        }

        public bool GodmasterEnemies {
            get => GetBool (false,EnemyRandomizerSettingsVars.GodmasterEnemies);
            set {
                StringValues[EnemyRandomizerSettingsVars.GodmasterEnemies] = "Godmaster Enemies";
                SetBool (value,EnemyRandomizerSettingsVars.GodmasterEnemies);
            }
        }
    }

    //Player specific settings
    public class EnemyRandomizerSaveSettings
    {
        public Dictionary<string, string> StringValues = new();
        private int seed;
        public int Seed {
            get => seed;
            set {
                StringValues[EnemyRandomizerSettingsVars.Seed] = "Seed (Click for new)";
                seed = value;
            }
        }
    }
}
