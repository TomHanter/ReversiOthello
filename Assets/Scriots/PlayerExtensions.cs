namespace Scriots
{
    public static class PlayerExtensions
    {
        public static Player Opponent(this Player player)
        {
            return player switch//с помощью switch case более читаемо
            {
                Player.Black => Player.White,
                Player.White => Player.Black,
                _ => Player.None
            };
        }
    }
}