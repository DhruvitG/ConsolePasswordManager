using System;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // Welcome message
            Console.WriteLine("Welcome to PassManager");

            PassManagerHelper passManagerHelper = new PassManagerHelper();
            
            bool accessGranted = accessGranted = passManagerHelper.authenticateUser();
            while (!accessGranted)
            {
                Console.WriteLine("Invalid Password. Please try again.");
                accessGranted = passManagerHelper.authenticateUser();
            }

            int optionSelected = -1;
            while(optionSelected != 5)
            {
                optionSelected = passManagerHelper.getMenuOption();
                switch (optionSelected)
                {
                    case 1:
                        passManagerHelper.viewAccounts();
                        break;
                    case 2:
                        passManagerHelper.addAccount();
                        break;
                    case 3:
                        passManagerHelper.deleteAccount();
                        break;
                    case 4:
                        passManagerHelper.editAccount();
                        break;
                    default:
                        break;
                }
            }   
                        
        }        
    }
}
