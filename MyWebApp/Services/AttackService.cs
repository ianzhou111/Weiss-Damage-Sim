using MyWebApp.Models;
using System.Reflection;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Annotations;
using OxyPlot.WindowsForms;

namespace MyWebApp.Services
{
    public class AttackService
    {
        public const int simulations = 10000;
        private List<Card> oppDeck;
        private List<Card> selfDeck;
        private List<Card> opp2ndDeck;
        public Damages? damages;

        // To keep track of the original DeckInfo for reinitialization in SimulateDamage
        private DeckInfo originalOppDeckInfo;
        private DeckInfo originalSelfDeckInfo;
        private DeckInfo originalOpp2ndDeckInfo;

        public AttackService()
        {
            oppDeck = new List<Card>();
            selfDeck = new List<Card>();
            opp2ndDeck = new List<Card>();
            damages = null;
        }

        public void InitializeDecks(DeckInfo oppDeckInfo, DeckInfo selfDeckInfo, DeckInfo opp2ndDeckInfo)
        {
            // Store the original DeckInfo for resetting in SimulateDamage
            originalOppDeckInfo = oppDeckInfo;
            originalSelfDeckInfo = selfDeckInfo;
            originalOpp2ndDeckInfo = opp2ndDeckInfo;

            // Initialize opponent's deck (oppDeck)
            oppDeck.Clear();
            DeckInitializer.InitializeOppDeck(oppDeck, oppDeckInfo);

            // Initialize self's deck (selfDeck)
            selfDeck.Clear();
            DeckInitializer.InitializeSelfDeck(selfDeck, selfDeckInfo);

            // Initialize opponent's second deck (opp2ndDeck)
            opp2ndDeck.Clear();
            DeckInitializer.InitializeOppDeck(opp2ndDeck, opp2ndDeckInfo);

            // Initialize the damages service with the created decks
            damages = new Damages(oppDeck, selfDeck, oppDeckInfo, selfDeckInfo, opp2ndDeckInfo);
        }

        public byte[] CalculateDamageAndGenerateGraph(AttackRequest request)
        {
            if (damages is null) throw new Exception("Need to initialize Deck First");
            // Implement attack damage calculation logic
            List<float> percentages = SimulateDamage(request.AttackNameValuePairs);

            // Create a new plot model
            var plotModel = new PlotModel();

            // Create a LineSeries for the damage percentages
            var lineSeries = new LineSeries
            {
                Title = "Damage Percentages",
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = OxyColors.Blue
            };

            // Add the damage threshold (1-14) and their corresponding percentages to the series
            for (int i = 0; i < percentages.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i + 1, percentages[i])); // Damage threshold starts at 1
            }

            // Add the series to the plot model
            plotModel.Series.Add(lineSeries);

            // Create a vertical line at damage 7
            var verticalLine = new LineAnnotation
            {
                Type = LineAnnotationType.Vertical,
                X = 7, // The x-coordinate for the vertical line (damage 7)
                Color = OxyColors.Red,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 2
            };
            plotModel.Annotations.Add(verticalLine);

            // Set the x-axis with larger label
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 1,
                Maximum = 14,
                MajorStep = 1,
                Title = "Remaining Clock",
                TitleFontSize = 36, // Increase x-axis label size
                FontSize = 12,
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White
            });

            // Set the y-axis with properly spaced dotted grid lines at multiples of 10
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 100,
                MajorStep = 10,
                MajorGridlineStyle = LineStyle.Dot, // Ensures grid lines appear at 10, 20, 30, etc.
                MajorGridlineColor = OxyColors.LightGray, // Faint grid lines
                Title = "Kill Percentage",
                TitleFontSize = 36, // Increase y-axis label size
                FontSize = 12,
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White
            });

            var stream = new MemoryStream();
            var pngExporter = new PngExporter { Width = 800, Height = 600, Resolution = 96 };
            pngExporter.Export(plotModel, stream);
            return stream.ToArray();
        }



        public List<float> SimulateDamage(List<AttackRequest.AttackNameValuePair> attackPairs)
        {
            List<int> results = new List<int>();

            // Simulate damage for the specified number of times
            for (int i = 0; i < simulations; i++)
            {
                // Reset the decks to their original state for each simulation
                var freshOppDeck = new List<Card>();
                var freshSelfDeck = new List<Card>();
                var freshOpp2ndDeck = new List<Card>();

                // Reinitialize the decks using the original input data
                // This can be done by calling InitializeDecks within the loop, or by directly using the original method in the constructor
                // Here we'll just call InitializeDecks again for each iteration
                InitializeDecks(originalOppDeckInfo, originalSelfDeckInfo, originalOpp2ndDeckInfo);

                int damage = 0;

                foreach (var attackPair in attackPairs)
                {
                    // Dynamically invoke the appropriate attack method using reflection
                    damage += CallDamageMethod(damages, attackPair.AttackName, attackPair.Value);
                }

                // Store the result of this simulation
                results.Add(damage);
            }

            // Calculate the percentages for various damage values
            List<int> damageThresholds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
            List<float> percentages = new List<float>();

            foreach (var dmg in damageThresholds)
            {
                // Get the number of results that are greater than or equal to the damage threshold
                var largerElements = results.Where(element => element >= dmg).ToList();
                float percentageOfElements = (float)largerElements.Count / results.Count * 100;
                percentages.Add(percentageOfElements);
            }

            return percentages;
        }

        private static int CallDamageMethod(Damages damages, string attackName, int value)
        {
            // Ensure that 'damages' is not null
            if (damages == null)
            {
                Console.WriteLine("Damages instance cannot be null.");
                return 0;
            }

            // Get the type of the Damages class
            Type damagesType = typeof(Damages);

            // Get the method information for the provided attackName
            MethodInfo methodInfo = damagesType.GetMethod(attackName, BindingFlags.Public | BindingFlags.Instance);

            if (methodInfo != null)
            {
                // Invoke the method dynamically with the provided value as an argument
                return (int)methodInfo.Invoke(damages, new object[] { value });
            }
            else
            {
                Console.WriteLine($"Method {attackName} not found in Damages.");
                return 0; // Return 0 if the method is not found
            }
        }
    }
}
