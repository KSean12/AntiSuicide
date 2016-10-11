using Rocket.API;

namespace AntiSuicide
{
    public class AntiSuicideConfiguration : IRocketPluginConfiguration
    {

        public bool Enabled { get; set; }

        public bool ResetSuicideCountAfterKick { get; set; }
        public byte MaxSuicidesBeforeKick { get; set; }

        public void LoadDefaults()
        {
                AntiSuicideConfiguration config = new AntiSuicideConfiguration();
                config.Enabled = true;
                config.MaxSuicidesBeforeKick = 3;
                config.ResetSuicideCountAfterKick = false;
            }
        }

        
    }

