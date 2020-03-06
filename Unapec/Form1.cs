using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Unapec
{
    public partial class UnapecForm : Form
    {
        public UnapecForm()
        {
            InitializeComponent();
        }

        private void BTNinitProcess_Click(object sender, EventArgs e)
        {
            if (!PerformValidations())
                return;

            ChromeDriver driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            
            try
            {
                string evaXpathExpression = XPathExpression.From("a").WhereAttribute("class", "nav-link").WhereAttribute("href", "http://eva.unapec.edu.do/").GetExpression();
                string evaLoginXpathExpression = XPathExpression.From("a").WhereAttribute("class", "btn-login").WhereAttribute("href", "https://eva.unapec.edu.do/moodle/auth/oidc/").GetExpression();

                //I could send it directly to EVA but to see the whole proccess from the Apec Main page
                driver.Navigate().GoToUrl("https://unapec.edu.do");

                //A little wait to secure everything loaded correctly
                System.Threading.Thread.Sleep(3000);

                IWebElement virtualClassElement = driver.FindElementByXPath(evaXpathExpression);
                virtualClassElement.Click();

                IWebElement btnLoginElement = driver.FindElementByXPath(evaLoginXpathExpression);
                btnLoginElement.Click();

                //Sleep 2 second just to wait for the transition in the form
                System.Threading.Thread.Sleep(2000);

                Login(driver);
                string filename = DownloadFile(driver);

                System.Threading.Thread.Sleep(10000);
                driver.Dispose();
                MessageBox.Show(string.Format("Se ha descargado el archivo:\n{0}", filename), "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                driver.Dispose();
                MessageBox.Show(string.Format("Ha ocurrido un error\nDescripcion:{0}",ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool PerformValidations()
        {
            if(string.IsNullOrWhiteSpace(TBXuserName.Text)){
                MessageBox.Show("El campo nombre de usuario no puede estar vacio","Alerta",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                TBXuserName.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TBXpassword.Text))
            {
                MessageBox.Show("El campo clave no puede estar vacio","Alerta",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                TBXpassword.Focus();
                return false;
            }

            return true;
        }

        private void Login(IWebDriver driver)
        {
            string emailInputXpathExpression = XPathExpression.From("input").WhereAttribute("type", "email").GetExpression();
            string passwordInputXpathExpression = XPathExpression.From("input").WhereAttribute("type", "password").GetExpression();
            string btnLoginXpathExpression = XPathExpression.From("input").WhereAttribute("type", "submit").GetExpression();
            string btnNoKeepSectionAliveXpathExpression = XPathExpression.From("input").WhereAttribute("value", "No").WhereAttribute("type", "button").GetExpression();

            IWebElement btnLoginInputElement = driver.FindElement(By.XPath(btnLoginXpathExpression));

            IWebElement emailInputElement = driver.FindElement(By.XPath(emailInputXpathExpression));
            emailInputElement.SendKeys(TBXuserName.Text);
            btnLoginInputElement.Click();

            //Sleep 2 second just to wait for the transition in the form
            System.Threading.Thread.Sleep(2000);

            IWebElement passwordInputElement = driver.FindElement(By.XPath(passwordInputXpathExpression));
            passwordInputElement.SendKeys(TBXpassword.Text);

            btnLoginInputElement = driver.FindElement(By.XPath(btnLoginXpathExpression));
            btnLoginInputElement.Click();

            //Sleep 2 second just to wait for the transition in the form
            System.Threading.Thread.Sleep(2000);

            IWebElement noKeepSectionAliveElement = driver.FindElement(By.XPath(btnNoKeepSectionAliveXpathExpression));
            noKeepSectionAliveElement.Click();
        }

        private string DownloadFile(IWebDriver driver)
        {
            string subjectXpathExpression = XPathExpression.From("a").WhereTextContains("ARQUITECTURA DE DESARROLLO CON TECNOLOGIA OPEN SOURCE - ISO910-41091-001").GetExpression();
            string powerPointFileXpathExpression = XPathExpression.From("a").WhereChild(XPathExpression.From("img").WhereAttributeContains("src", "powerpoint")).GetExpression();
            string fileName = null;

            IWebElement subjectElement = driver.FindElement(By.XPath(subjectXpathExpression));
            subjectElement.Click();

            IWebElement powerPointElement = driver.FindElement(By.XPath(powerPointFileXpathExpression));
            fileName = powerPointElement.Text;
            powerPointElement.Click();

            return fileName;
        }
    }
}
