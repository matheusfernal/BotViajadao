namespace BotViajadao.Model.Yelp
{
    public static class BusisnessExtensions
    {
        public static string ObtemSubtituloCard(this Business busisness)
        {
            return $"{busisness.ReviewCount} avaliações com média de {busisness.Rating} {Util.Emoji.Star}";
        }
    }
}