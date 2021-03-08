using System;
using System.Collections.Generic;

namespace Dealer
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] Cities = new string[] { "Lubeck", "Reval", "Bergen" };
            for (int i = 0; i < Cities.Length; i++)
            {
                List<ProductsCity> ProductsCity = new List<ProductsCity>
                {
                    new ProductsCity(20, 50, 60, 36, 96, Cities[0]),
                    new ProductsCity(35, 50, 40, 60, 45, Cities[1]),
                    new ProductsCity(62, 15, 18, 82, 54, Cities[2])
                };
                Merchants merchants = new Merchants(ProductsCity, 50);

                Console.WriteLine("Run " + (i + 1));
                Console.WriteLine("Initial coins: " + merchants.Coins);
                string city = Cities[i];
                do
                {
                    Merchants.CurrentPurchase cp = merchants.Buy(city);
                    Console.WriteLine($"Buy {cp.Name} for {cp.CostBuy} coins in {cp.From}. {merchants.Coins} coins left");
                    city = cp.To;
                    merchants.Sell(cp);
                    Console.WriteLine($"Sell {cp.Name} for {cp.CostSell} coins in {cp.To}. {merchants.Coins} coins left");
                }
                while (merchants.CountCities != 1);
                Console.WriteLine("Final coins: " + merchants.Coins);
                ProductsCity.Clear();
            }
            Console.ReadKey();
        }
    }
    public class Merchants
    {
        private Dictionary<string, int[]> Table;//0-2 column - cities, 3 - maximum indexes, 4 - profit
        public int Coins { get; set; }
        public int CountCities
        {
            get
            {
                return Table.Count;
            }
        }
        private List<ProductsCity> productsCity;

        public Merchants(List<ProductsCity> products, int Coins)
        {
            productsCity = products;
            Table = new Dictionary<string, int[]>();
            foreach (var product in products)
                Table[product.City] = new int[]
                {
                    product.Salt,
                    product.Fish,
                    product.Cloth,
                    product.Copper,
                    product.Furs
                };
            this.Coins = Coins;
        }
        /// <summary>
        /// product purchase 
        /// </summary>
        /// <param name="currentCity"></param>
        /// <returns></returns>
        public CurrentPurchase Buy(string currentCity)
        {
            CurrentPurchase cp = Profit(currentCity);
            Coins -= cp.CostBuy;
            Table.Remove(currentCity);
            return cp;
        }
        /// <summary>
        /// product sale 
        /// </summary>
        /// <param name="cp"></param>
        public void Sell(CurrentPurchase cp)
        {
            Coins += cp.CostSell;
        }
        /// <summary>
        /// information about a product that is profitable to buy and sell
        /// </summary>
        /// <param name="currentCity"></param>
        /// <returns></returns>
        public CurrentPurchase Profit(string currentCity)
        {
            string[] Cities;
            int[,] Matrix = GetMatrixProducts(currentCity, out Cities);
            int indexCurrentCity = GetCurrentIndex(Cities, currentCity);
            Matrix = SetMaxIndexes(Matrix);
            for (int i = 0; i < Matrix.GetLength(0); i++)
                Matrix[i, Matrix.GetLength(1) - 1] = Matrix[i, Matrix[i, Matrix.GetLength(1) - 2]] - Matrix[i, indexCurrentCity];

            int profitProductIndex = MaxC(Matrix.GetLength(1) - 1, Matrix);

            CurrentPurchase current = new CurrentPurchase(productsCity[0].NameListProduct[profitProductIndex],
                Convert.ToInt32(Table[currentCity].GetValue(profitProductIndex)),
                Convert.ToInt32( Table[Cities[Matrix[profitProductIndex, Matrix.GetLength(1) - 2]]].GetValue(profitProductIndex)),
                currentCity, Cities[Matrix[profitProductIndex, Matrix.GetLength(1) - 2]]);

            return current;
        }
        /// <summary>
        /// Get the current city index 
        /// </summary>
        /// <param name="Cities"></param>
        /// <param name="currentCity"></param>
        /// <returns></returns>
        private int GetCurrentIndex(string[] Cities, string currentCity)
        {
            int index = 0;
            for (int i = 0; i < Cities.Length; i++)
                if (Cities[i] == currentCity)
                    index = i;
            return index;
        }
        /// <summary>
        /// entering the data of the prices of products into the table
        /// </summary>
        /// <param name="City"></param>
        /// <param name="cities"></param>
        /// <returns></returns>
        private int[,] GetMatrixProducts(string City, out string[] cities)
        {
            int[,] ProductsAndProfit = new int[5, Table.Count + 2];
            int i = 0;
            cities = new string[Table.Count];
            foreach (var obj in Table)
            {
                cities[i] = obj.Key;
                int j = 0;
                foreach (var cell in obj.Value)
                {
                    ProductsAndProfit[j, i] = cell;
                    j++;
                }
                i++;
            }
            return ProductsAndProfit;
        }
        /// <summary>
        /// search for indexes of maximum values 
        /// </summary>
        /// <param name="Matrix"></param>
        /// <returns></returns>
        private int[,] SetMaxIndexes(int[,] Matrix)
        {
            for (int i = 0; i < Matrix.GetLength(0); i++)
                Matrix[i, Matrix.GetLength(1) - 2] = MaxR(i, Matrix);
            return Matrix;
        }
        /// <summary>
        /// finding the maximum value by row
        /// </summary>
        /// <param name="NumRow"></param>
        /// <param name="Matrix"></param>
        /// <returns></returns>
        private int MaxR(int NumRow, int[,] Matrix)
        {
            int maxElement = Matrix[NumRow, 0], maxIndex = 0;
            for (int i = 0; i < Matrix.GetLength(1) - 2; i++)
                if (maxElement < Matrix[NumRow, i])
                {
                    maxElement = Matrix[NumRow, i];
                    maxIndex = i;
                }
            return maxIndex;
        }
        /// <summary>
        /// finding the maximum value by column 
        /// </summary>
        /// <param name="NumCollumn"></param>
        /// <param name="Matrix"></param>
        /// <returns></returns>
        private int MaxC(int NumCollumn, int[,] Matrix)
        {
            int maxElement = Matrix[0, NumCollumn], maxIndex = 0;
            for (int i = 0; i < Matrix.GetLength(0); i++)
                if (maxElement < Matrix[i, NumCollumn])
                {
                    maxElement = Matrix[i, NumCollumn];
                    maxIndex = i;
                }
            return maxIndex;
        }

        public class CurrentPurchase
        {
            public CurrentPurchase(string Name, int CostBuy, int CostSell, string From, string To)
            {
                this.Name = Name;
                this.CostBuy = CostBuy;
                this.CostSell = CostSell;
                this.From = From;
                this.To = To;
            }
            public int CostBuy { get; private set; }
            public int CostSell { get; private set; }
            public string Name { get; private set; }
            public string From { get; private set; }
            public string To { get; private set; }
        }    
    }
    public class ProductsCity
    {
        public ProductsCity(int Salt, int Fish, int Cloth, int Copper, int Furs, string City)
        {
            this.Salt = Salt;
            this.Fish = Fish;
            this.Cloth = Cloth;
            this.Copper = Copper;
            this.Furs = Furs;
            this.City = City;
        }
        public string City { get; private set; }
        public int Salt { get; private set; }
        public int Fish { get; private set; }
        public int Cloth { get; private set; }
        public int Copper { get; private set; }
        public int Furs { get; private set; }
        public readonly string[] NameListProduct = new string[] { "salt", "fish", "cloth", "copper", "furs" };
    }
}
