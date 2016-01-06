using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Collections;
using System.IO;
using System.Security;
using System.Runtime.InteropServices;

namespace PasswordManager
{
    public class PassManagerHelper
    {
        private string masterPassword;
        private string myDocPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string encryptedFilePath;
        private string decryptedFilePath;
        
        public PassManagerHelper()
        {
            this.encryptedFilePath = this.myDocPath + @"\encrypted_passlist.txt";
            this.decryptedFilePath = this.myDocPath + @"\decrypted_passlist.txt";
        }

        public bool authenticateUser()
        {
            byte[] byte_real_password = Encoding.ASCII.GetBytes("password");
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] realHashValue = mySHA256.ComputeHash(byte_real_password);

            Console.Write("Please enter your master password: ");
            string enteredPassword = this.getPassword();
            byte[] byte_password = Encoding.ASCII.GetBytes(enteredPassword);
            byte[] hashValue = mySHA256.ComputeHash(byte_password);

            bool isEqual = StructuralComparisons.StructuralEqualityComparer.Equals(realHashValue, hashValue);
            if (isEqual)
            {
                this.masterPassword = enteredPassword;
            }
            return isEqual;
        }

        public int getMenuOption()
        {
            Console.WriteLine("Main Menu");
            Console.WriteLine("=========");
            Console.WriteLine("1) View your saved accounts");
            Console.WriteLine("2) Add an account");
            Console.WriteLine("3) Delete an account");
            Console.WriteLine("4) Edit an account");
            Console.WriteLine("5) Exit");
            Console.WriteLine("Enter a number for the corresponding menu option:");
            string option = Console.ReadLine();
            int menuOptionEntered = int.Parse(option);
            return menuOptionEntered;
        }

        public void addAccount()
        {
            if (!File.Exists(this.encryptedFilePath))
            {
                using (StreamWriter outputFile = new StreamWriter(this.myDocPath + @"\temp_file.txt"))
                {
                    outputFile.WriteLine("List of Accounts");
                }
                Cryptology.EncryptFile(this.myDocPath + @"\temp_file.txt", this.encryptedFilePath, this.masterPassword);
                File.Delete(this.myDocPath + @"\temp_file.txt");
            }
            Cryptology.DecryptFile(this.encryptedFilePath, this.decryptedFilePath, this.masterPassword);
            Console.Write("Please enter the title of the account you wish to add: ");
            string accountTitle = Console.ReadLine();
            Console.Write("Please enter the username for this account: ");
            string username = Console.ReadLine();
            Console.Write("Please enter the password for this account: ");
            string accountPassword = Console.ReadLine();
            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(this.decryptedFilePath, true))
            {
                outputFile.WriteLine(accountTitle + ", " + username + ", " + accountPassword);
            }
            Cryptology.EncryptFile(this.decryptedFilePath, this.encryptedFilePath, this.masterPassword);
            //File.Delete(this.decryptedFilePath);
            Console.WriteLine("Account added successfully");
        }

        public void viewAccounts()
        {            
            if (!File.Exists(this.encryptedFilePath))
            {
                Console.WriteLine("No accounts exist. Please add an account from the main menu.");
                return;
            }
            Cryptology.DecryptFile(this.encryptedFilePath, this.decryptedFilePath, this.masterPassword);

            string[] lines = File.ReadAllLines(this.decryptedFilePath);
            
            if (lines.Length < 2)
            {
                Console.WriteLine("No accounts exist. Please add an account from the main menu.");
            }
            else
            {
                string[][] accounts = new string[lines.Length][];
                for (int i = 1; i < lines.Length; i++)
                {
                    accounts[i] = lines[i].Split(',');
                }
                for (int i = 1; i < accounts.Length; i++)
                {
                    Console.Write(i + ") ");
                    Console.WriteLine(accounts[i][0]);
                }

                //read user input for which account they want credentials for
                Console.Write("Please select an account by entering its corresponding number: ");
                string option = Console.ReadLine();
                int accountSelected = int.Parse(option);
                while (!(accountSelected > 0 && accountSelected < accounts.Length))
                {
                    Console.Write("Please enter a valid option: ");
                    option = Console.ReadLine();
                    accountSelected = int.Parse(option);
                }
                Console.WriteLine(accounts[accountSelected][0] + " Account Details:");
                Console.WriteLine("Username: " + accounts[accountSelected][1]);
                Console.WriteLine("Password: " + accounts[accountSelected][2]);
            }
            

            //File.Delete(this.decryptedFilePath);
        }

        public void deleteAccount()
        {            
            if (!File.Exists(this.encryptedFilePath))
            {
                Console.WriteLine("No accounts exist. Please add an account from the main menu.");
                return;
            }
            Cryptology.DecryptFile(this.encryptedFilePath, this.decryptedFilePath, this.masterPassword);
                    
            string[] lines = File.ReadAllLines(this.decryptedFilePath);

            File.Delete(this.decryptedFilePath);           
            if (lines.Length < 2)
            {
                Console.WriteLine("No accounts exist. Please add an account from the main menu.");
            }else
            {
                string[][] accounts = new string[lines.Length][];
                for (int i = 1; i < lines.Length; i++)
                {
                    accounts[i] = lines[i].Split(',');
                }
                for (int i = 1; i < accounts.Length; i++)
                {
                    Console.Write(i + ") ");
                    Console.WriteLine(accounts[i][0]);
                }

                //read user input for which account they want credentials for
                Console.Write("Please select an account by entering its corresponding number: ");
                string option = Console.ReadLine();
                int accountSelected = int.Parse(option);

                while (!(accountSelected > 0 && accountSelected < accounts.Length))
                {
                    Console.Write("Please enter a valid option: ");
                    option = Console.ReadLine();
                    accountSelected = int.Parse(option);
                }               

                using (StreamWriter streamWriter = new StreamWriter(this.decryptedFilePath))
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i != accountSelected)
                        {
                            Console.WriteLine(lines[i]);
                            streamWriter.WriteLine(lines[i]);
                        }
                    }
                }
                Console.WriteLine("Account deleted successfully");

                Cryptology.EncryptFile(this.decryptedFilePath, this.encryptedFilePath, this.masterPassword);
            }
            
            //File.Delete(this.decryptedFilePath);
        }

        private string getPassword()
        {
            string pwd = "";
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd = pwd.Substring(1, pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd += i.KeyChar;
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return pwd;
        }

        public void editAccount()
        {

        }
    }


    
}





