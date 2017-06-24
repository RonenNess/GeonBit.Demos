
namespace SpaceshipGame
{
    /// <summary>
    /// A static class to hold & manage player current status (hp, score, ammo, etc..).
    /// This class also connects to the UI elements that show the status and update them.
    /// </summary>
    static class PlayerStatus
    {
        /// <summary>
        /// The entity used to display player current hp.
        /// </summary>
        public static GeonBit.UI.Entities.ProgressBar HpShow;

        /// <summary>
        /// Get / set player max hp.
        /// </summary>
        public static int MaxHp
        {
            get { return (int)HpShow.Max; }
            set { HpShow.Max = HpShow.StepsCount = (uint)value; }
        }

        /// <summary>
        /// Get / set player current hp.
        /// </summary>
        public static int Hp
        {
            get { return HpShow.Value; }
            set { HpShow.Value = value; }
        }

        /// <summary>
        /// The entity used to display player current ammo.
        /// </summary>
        public static GeonBit.UI.Entities.ProgressBar AmmoShow;

        /// <summary>
        /// Get / set player max ammo.
        /// </summary>
        public static int MaxAmmo
        {
            get { return (int)AmmoShow.Max; }
            set { AmmoShow.Max = AmmoShow.StepsCount = (uint)value; }
        }

        /// <summary>
        /// Get / set player current ammo.
        /// </summary>
        public static int Ammo
        {
            get { return AmmoShow.Value; }
            set { AmmoShow.Value = value; }
        }

        /// <summary>
        /// The entity used to display player current score.
        /// </summary>
        public static GeonBit.UI.Entities.Paragraph ScoreShow;

        // store current score
        static int _score = 0;

        /// <summary>
        /// Get / set player current score.
        /// </summary>
        public static int Score
        {
            get { return _score; }
            set { _score = value; ScoreShow.Text = "Score: " + _score.ToString(); }
        }

        /// <summary>
        /// Reset player status.
        /// </summary>
        public static void Reset()
        {
            // set hp
            MaxHp = 5;
            Hp = MaxHp;

            // set ammo
            MaxAmmo = 5;
            Ammo = 0;

            // zero score
            Score = 0;
        }
    }
}
