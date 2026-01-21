using LearnC_.MockData;

namespace LearnC_.MockData
{
  public class MockMain
  {
        public static void Main(string[] args)
        {
            Console.WriteLine(MockData.Products.Select(p => p.Name).OrderByDescending(p => p.Price).Take(3));
        }
  }
}
