using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WpfChangeCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static decimal itemPrice, amountGiven, change;
        // coinInfo array corresponds to quarters, dimes, nickels, pennies
        //  first set holds the number of that coin type
        //  second set holds the coin value
        static decimal[,] coinInfo = 
            { {0, 0, 0, 0 } , { .25M, .10M, .05M, .01M } };
        static int coinInfoIndexNum = 0, coinInfoIndexVal = 1;
        // array of coin type labels
        static string[] coinTypeLabels =
            {"Quarters: ", "Dimes: ", "Nickels: ", "Pennies: "};      

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            bool inputAmountsValid;

            Label[] labelCoins = { labelQuarters, labelDimes, labelNickels, labelPennies };          

            try
            {
                // Validate the input amounts
                inputAmountsValid = validateInputAmounts();

                // If the amounts are valid, 
                //   calculate the change and display the result
                if (inputAmountsValid)
                {
                    calcCoinAmounts();

                    displayResults(labelCoins);
                }
            }
            catch (Exception exceptionObject)
            {
                MessageBox.Show("An error occurred: " + exceptionObject.Message);
            }           

        }


        // If the price is changed, clear the corresponding error label 
        private void textBoxItemPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            labelPriceError.Content = "";
        }

        // If the amount given is changed, clear the corresponding error label
        private void textBoxAmountFromCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            labelAmountGivenError.Content = "";
        }


        // If Clear All button is clicked,
        //  clear the price, amount given, corresponding error labels,
        //  and all change amounts
        private void buttonClearAll_Click(object sender, RoutedEventArgs e)
        {
            textBoxItemPrice.Text = "";
            textBoxAmountFromCustomer.Text = "";
            labelPriceError.Content = "";
            labelAmountGivenError.Content = "";
            labelChange.Content = "Change: ";           
            labelQuarters.Content = "Quarters: ";
            labelNickels.Content = "Dimes: ";
            labelDimes.Content = "Nickels: ";
            labelPennies.Content = "Pennies: ";
        }

        // If Clear button for price is clicked,
        //  clear the price and corresponding error label
        private void buttonClearPrice_Click(object sender, RoutedEventArgs e)
        {
            textBoxItemPrice.Text = "";
            labelPriceError.Content = "";
        }

        // If Clear button for amount given is clicked,
        //  clear the amount given and corresponding error label
        private void buttonClearAmount_Click(object sender, RoutedEventArgs e)
        {
            textBoxAmountFromCustomer.Text = "";
            labelAmountGivenError.Content = "";
        }


        // Validate input amounts
        // Return true if both are valid, otherwise return false
        private bool validateInputAmounts()
        {
            bool result = false; 
                     
            // Validate the item price
            bool itemPriceValid = validateMoney(textBoxItemPrice, labelPriceError, out itemPrice);

            // Validate the amount received from the customer
            bool AmountGivenValid = 
               validateMoney(textBoxAmountFromCustomer, labelAmountGivenError, out amountGiven);

            // If price and amount are valid numbers, 
            //   validate that amount given is greater than or equal to price
            if (itemPriceValid && AmountGivenValid)
            {                            
                if (amountGiven >= itemPrice)
                {
                    result = true;
                }
                else
                {
                    labelAmountGivenError.Content =
                        "Please enter a number greater than or equal to the item price.";                  
               }
            }
            return result;
        }

        // Calculate the change to give, and the 
        //   number needed of each type of coin in the change,
        //   storing results in coinInfo array 
        private void calcCoinAmounts()
        {
            // Calculate the change amount
            change = amountGiven  - itemPrice;
            decimal changeLeft = change;

            // For each coin type, calculate the number of coins needed:
            //  it is the change left divided by coin value, rounded down to nearest integer
            for (int i = 0; i < coinInfo.GetLength(1); i++)
            {
               coinInfo[coinInfoIndexNum, i] =
                  Decimal.Floor(changeLeft / coinInfo[coinInfoIndexVal, i]);
                // Update the change left to give
                changeLeft = changeLeft % coinInfo[coinInfoIndexVal, i];
            }
       }


        // Display the change needed in the change label,
        // and display the amoumt of each coin type in the label array,
        private void displayResults(Label [] labelCoins)
        {
            labelChange.Content = "Change: " + String.Format("{0:C2}", change);
            for (int i = 0; i < labelCoins.Length; i++)
            {
                labelCoins[i].Content = 
                    coinTypeLabels[i] + coinInfo[coinInfoIndexNum, i];
            }
        }


        // Validate currency amount, allowing up to 2 decimal places
        // Input text box containing the number, and label for error message
        private bool validateMoney(TextBox inputTextBox, Label labelError, out decimal resultAmount)
        {
            bool amountValid = validatePosDecNum(inputTextBox, labelError, out resultAmount);
            if (amountValid)
            {
                // Check if the number of decimal places is correct           
               if (resultAmount != Math.Round(resultAmount, 2))
               {
                    labelError.Content = 
                                "Please enter a number with no more than 2 decimal places.";
                    amountValid = false;
                }
            }          
            return amountValid;
        }


        // Validate a decimal number from the textbox that must be greater than 0
        // Return true if decimal number is valid, otherwise return false
        // If decimal number is invalid, place appropriate error message in label
        private bool validatePosDecNum(TextBox inputTextBox, Label labelError, 
                                                            out decimal resultNum)
        {
            bool numValid = false;
            // Attempt to convert textbox text to decimal
            // If conversion is successful, check that it is positive
            if (Decimal.TryParse(inputTextBox.Text, out resultNum))
            {
                if (resultNum > 0)
                {
                    numValid = true;
                }
                else
                {
                    labelError.Content = "Please enter a positive number.";
                }
            }
            else
            {
                labelError.Content = "Please enter a valid number that is not too large.";
           }
           return numValid;
        }

    }
}
