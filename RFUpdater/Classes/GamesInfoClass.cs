using System;

namespace RFUpdater
{
    public class GamesInfoClass
    {
        public string GameName { get; set; }

        public string GamePictureUri { get; set; }
        public string InfoDriveLocationUri { get; set; }
        public string GameDriveLocationUri { get; set; }
        public string GamePCLocation { get; set; }

        public Version CurrentGameVersion { get; set; }
        public Version NewGameVersion { get; set; }
        public int GameStatus { get; set; }
        public int GameReleaseStatus { get; set; }
        public int Tag { get; set; }

    }
}
