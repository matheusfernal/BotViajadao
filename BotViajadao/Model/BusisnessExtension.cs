namespace BotViajadao.Model
{
    public static class BusisnessExtension
    {
        public static string ObtemSubtituloCard(this Business busisness)
        {
            return $"{busisness.ReviewCount} avaliações com média de {busisness.Rating} {Util.Emoji.Star}";
        }
    }
}