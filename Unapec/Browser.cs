using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sel
{
    /// <summary>
    /// A class That provide some methods to find Web elements in selenium
    /// </summary>
    public static class Browser
    {
        /// <summary>
        /// Get the value of an input element
        /// </summary>
        /// <param name="inputElement">Input element to look for the value</param>
        public static string GetInputValue(IWebElement inputElement)
        {

            if (inputElement.TagName != "input")
                throw new Exception("The element is not an input");
            return inputElement.GetAttribute("value");
        }

        /// <summary>
        /// Get the checkState of the checkbox or radio element
        /// </summary>
        /// <param name="inputElement">Input element to look for the check state</param>
        public static bool GetCheckState(IWebElement inputElement)
        {
            if (!new string[] { "checkbox", "radio" }.Contains(inputElement.GetAttribute("type")))
                throw new Exception("The element is not a chackbox or radio button");

            return inputElement.GetAttribute("checked") == null ? false : true;
        }

        public static bool DisableAlert(IWebDriver driver)
        {
            string jsFunction = "window.oldAlert = window.alert;window.alert = function(){};return true";
            object result = ((IJavaScriptExecutor)driver).ExecuteScript(jsFunction);
            return (bool)result;
        }

        public static bool RestoreAlert(IWebDriver driver)
        {
            string jsFunction = @"if(window.oldAlert != undefined){window.alert = window.oldAlert; delete window.oldAlert; return window.oldAlert == undefined}else {return true}";
            object result = ((IJavaScriptExecutor)driver).ExecuteScript(jsFunction);
            return (bool)result;
        }

        public static IWebElement GetElementByJsXpathExpresion(IWebDriver driver, string XpathExpresion)
        {
            string jsFunction = "let results = [];let query = document.evaluate(`" + XpathExpresion + "`, document,null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);for (let i = 0, length = query.snapshotLength; i < length; ++i) {results.push(query.snapshotItem(i));} if(results.length > 0) return results[0];else return null";
            object currentElement = ((IJavaScriptExecutor)driver).ExecuteScript(jsFunction);
            return currentElement == null ? null : (IWebElement)currentElement;
        }

        public static IEnumerable<IWebElement> GetElementsByJsXpathExpresion(IWebDriver driver, string XpathExpresion)
        {
            string jsFunction = "let results = [];let query = document.evaluate(`" + XpathExpresion + "`, document,null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);for (let i = 0, length = query.snapshotLength; i < length; ++i) {results.push(query.snapshotItem(i));} if(results.length > 0) return results;else return null";
            object currentElement = ((IJavaScriptExecutor)driver).ExecuteScript(jsFunction);
            return currentElement == null ? null : (IEnumerable<IWebElement>)currentElement;
        }
    }

    /// <summary>
    /// An interface to provide method for condition formats
    /// </summary>
    public interface IXpathExpression
    {
        /// <summary>
        /// Return a string representation of an xpath expression with a default // selecting node
        /// </summary>
        string GetExpression();
        /// <summary>
        /// Return a string representation of an xpath expression without a selecting node
        /// </summary>
        string GetPartialExpression();
    }

    /// <summary>
    /// A class to create xpath expressions
    /// </summary>
    public class XPathExpression : IXpathExpression
    {
        private string _expression;
        private readonly string _tagName;
        private Queue<string> collectionOfAttributeExpression;
        private Queue<string> collectionOfPartialExpression;
        private static string[] operators = new string[] { "and", "or" };
        public string defaultPathExpression = "//";

        /// <summary>
        /// Creates a new xpath expression for the specified tag
        /// </summary>
        /// <param name="tagname">Name of the tag to look for</param>
        public XPathExpression(string tagname = null)
        {
            _expression = string.Empty;
            _tagName = tagname ?? string.Empty;
            collectionOfAttributeExpression = new Queue<string>();
            collectionOfPartialExpression = new Queue<string>();
        }

        /// <summary>
        /// Return a string representation of an xpath expression with a default // selecting node
        /// </summary>
        public string GetExpression()
        {
            string firstSlash = collectionOfPartialExpression.Count > 0 ? "/" : string.Empty;
            if (collectionOfAttributeExpression.Count > 0)
                _expression = string.Format("{0}{1}[{2}]{3}{4}", defaultPathExpression, _tagName, string.Join(" ", collectionOfAttributeExpression), firstSlash, string.Join("/", collectionOfPartialExpression));
            else
                _expression = string.Format("{0}{1}{2}{3}", defaultPathExpression, _tagName, firstSlash, string.Join("/", collectionOfPartialExpression));
            return _expression;
        }

        /// <summary>
        /// Return a string representation of an xpath expression without a selecting node
        /// </summary>
        public string GetPartialExpression()
        {
            string firstSlash = collectionOfPartialExpression.Count > 0 ? "/" : string.Empty;
            if (collectionOfAttributeExpression.Count > 0)
                _expression = string.Format("{0}[{1}]{2}{3}", _tagName, string.Join(" ", collectionOfAttributeExpression), firstSlash, string.Join("/", collectionOfPartialExpression));
            else
                _expression = string.Format("{0}{1}{2}", _tagName, firstSlash, string.Join("/", collectionOfPartialExpression));
            return _expression;
        }

        /// <summary>
        /// Add an operator at the end of the expression if doesnt have one
        /// </summary>
        private void AddDefaultOperator()
        {
            if (collectionOfAttributeExpression.Count > 0)
            {
                string lastValue = collectionOfAttributeExpression.LastOrDefault();
                if (lastValue != null && !operators.Contains(lastValue))
                    collectionOfAttributeExpression.Enqueue(operators[0]);
            }
        }

        /// <summary>
        /// Checks if the current expression ends with an operator
        /// </summary>
        /// <returns></returns>
        private bool HasOperator()
        {
            if (collectionOfAttributeExpression.Count > 0)
            {
                string lastValue = collectionOfAttributeExpression.LastOrDefault();
                if (operators.Contains(lastValue))
                    return true;
                else
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Creates an expression with the expecified attributeName and value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="attributeValue">The value to look for the attribute</param>
        public XPathExpression WhereAttribute(string attributeName, string attributeValue = null)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            string expresion = attributeValue == null
                ? string.Format("{0}", attributeName, attributeValue)
                : string.Format("{0}={1}", attributeName, attributeValue.Contains("'") ? string.Format("\"{0}\"", attributeValue) : string.Format("'{0}'", attributeValue));

            collectionOfAttributeExpression.Enqueue(expresion);
            return this;
        }

        /// <summary>
        /// Creates an expression with the expecified attributeName and value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="attributeValue">The value to look for attributes that are not equal to</param>
        public XPathExpression WhereNotAttribute(string attributeName, string attributeValue = null)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            string expresion = attributeValue == null
                ? string.Format("not({0})", attributeName, attributeValue)
                : string.Format("not({0}={1})", attributeName, attributeValue.Contains("'") ? string.Format("\"{0}\"", attributeValue) : string.Format("'{0}'", attributeValue));

            collectionOfAttributeExpression.Enqueue(expresion);
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes with number and look for one that is greater than the especified attribute value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="attributeValue">The value as number to compare</param>
        public XPathExpression WhereGreaterThan(string attributeName, long attributeValue)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            collectionOfAttributeExpression.Enqueue(string.Format("{0}>{1}", attributeName, attributeValue));
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes with number and look for one that is greater or equal than the especified attribute value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="attributeValue">The value as number to compare</param>
        public XPathExpression WhereGreaterOrEqual(string attributeName, long attributeValue)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            collectionOfAttributeExpression.Enqueue(string.Format("{0}>={1}", attributeName, attributeValue));
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes with number and look for one that is lower than the especified attribute value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="attributeValue">The value as number to compare</param>
        /// <returns></returns>
        public XPathExpression WhereLowerThan(string attributeName, long attributeValue)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            collectionOfAttributeExpression.Enqueue(string.Format("{0}<{1}", attributeName, attributeValue));
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes with number and look for one that is lower or equal than the especified attribute value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="attributeValue">The value as number to compare</param>
        /// <returns></returns>
        public XPathExpression WhereLowerOrEqual(string attributeName, long attributeValue)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            collectionOfAttributeExpression.Enqueue(string.Format("{0}<={1}", attributeName, attributeValue));
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that contains the expecified text
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="textValue"></param>
        public XPathExpression WhereAttributeContains(string attributeName, string textValue)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            collectionOfAttributeExpression.Enqueue(string.Format("contains({0},{1})", attributeName, textValue.Contains("'") ? string.Format("\"{0}\"", textValue) : string.Format("'{0}'", textValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that not contains the expecified text
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="textValue"></param>
        public XPathExpression WhereAttributeNotContains(string attributeName, string textValue)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            collectionOfAttributeExpression.Enqueue(string.Format("not(contains({0},{1}))", attributeName, textValue.Contains("'") ? string.Format("\"{0}\"", textValue) : string.Format("'{0}'", textValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that starts with the expecified text
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="textValue"></param>
        public XPathExpression WhereAttributeStartsWith(string attributeName, string textValue)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            collectionOfAttributeExpression.Enqueue(string.Format("starts-with({0},{1})", attributeName, textValue.Contains("'") ? string.Format("\"{0}\"", textValue) : string.Format("'{0}'", textValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that do not starts with the expecified text
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="textValue"></param>
        public XPathExpression WhereAttributeNotStartsWith(string attributeName, string textValue)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            collectionOfAttributeExpression.Enqueue(string.Format("not(starts-with({0},{1}))", attributeName, textValue.Contains("'") ? string.Format("\"{0}\"", textValue) : string.Format("'{0}'", textValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that ends with the expecified text *only valid in xpath 2.0*
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="textValue"></param>
        public XPathExpression WhereAttributeEndsWith(string attributeName, string textValue)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            collectionOfAttributeExpression.Enqueue(string.Format("ends-with({0},{1})", attributeName, textValue.Contains("'") ? string.Format("\"{0}\"", textValue) : string.Format("'{0}'", textValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that do not ends with the expecified text *only valid in xpath 2.0*
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="textValue"></param>
        public XPathExpression WhereAttributeNotEndsWith(string attributeName, string textValue)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            collectionOfAttributeExpression.Enqueue(string.Format("not(ends-with({0},{1}))", attributeName, textValue.Contains("'") ? string.Format("\"{0}\"", textValue) : string.Format("'{0}'", textValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text to match with the expecified text value
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        public XPathExpression WhereTextEqual(string textValue)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("text()={0}", textValue.Contains("'") ? string.Format("\"{0}\"", textValue) : string.Format("'{0}'", textValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag number greater than the expecified text value
        /// </summary>
        /// <param name="number">Number to compare</param>
        public XPathExpression WhereTextGreaterThan(double number)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("number(text())>{0}", number));
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag number greater or equal than the expecified text value
        /// </summary>
        /// <param name="number">Text value to look for</param>
        public XPathExpression WhereTextGreaterOrEqualThan(double number)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("number(text())>={0}", number));
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag number lower than the expecified text value
        /// </summary>
        /// <param name="number">Text value to look for</param>
        public XPathExpression WhereTextLowerThan(double number)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("number(text())<{0}", number));
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag number lower or equal than the expecified text value
        /// </summary>
        /// <param name="number">Text value to look for</param>
        public XPathExpression WhereTextLowerOrEqualThan(double number)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("number(text())<={0}", number));
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that contains the expecified text value
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        public XPathExpression WhereTextContains(string textValue)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("contains(text(),{0})", textValue.Contains("'") ? string.Format("\"{0}\"", textValue) : string.Format("'{0}'", textValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that starts with the expecified text value
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        public XPathExpression WhereTextStartsWith(string textValue)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("starts-with(text(),{0})", textValue.Contains("'") ? string.Format("\"{0}\"", textValue) : string.Format("'{0}'", textValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that do not contains the expecified text value
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        public XPathExpression WhereTextNotContains(string textValue)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("not(contains(text(),{0}))", textValue.Contains("'") ? string.Format("\"{0}\"", textValue) : string.Format("'{0}'", textValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that do not starts with the expecified text value
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        public XPathExpression WhereTextNotStartsWith(string textValue)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("not(starts-with(text(),{0}))", textValue.Contains("'") ? string.Format("\"{0}\"", textValue) : string.Format("'{0}'", textValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text to not match with the expecified text value
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        /// <returns></returns>
        public XPathExpression WhereTextNotEqual(string textValue)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("not(text()={0})", textValue.Contains("'") ? string.Format("\"{0}\"", textValue) : string.Format("'{0}'", textValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression for the current tag name that look for one that does not have an inner text
        /// </summary>
        public XPathExpression WhereTextIsEmpty()
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("not(contains(text(), '(default)'))"));
            return this;
        }

        /// <summary>
        /// Creates an expression for the current tag name that look for one that have an inner text
        /// </summary>
        public XPathExpression WhereTextIsNotEmpty()
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("contains(text(), '(default)')"));
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that match a regex expression *only valid in xpath 2.0*
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="textValue"></param>
        public XPathExpression WhereAttributeMatch(string attributeName, string regexExpression)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            collectionOfAttributeExpression.Enqueue(string.Format("matches({0},{1})", attributeName, regexExpression.Contains("'") ? string.Format("\"{0}\"", regexExpression) : string.Format("'{0}'", regexExpression)));
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that do not match a regex expression *only valid in xpath 2.0*
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="textValue"></param>
        public XPathExpression WhereAttributeNotMatch(string attributeName, string regexExpression)
        {
            AddDefaultOperator();
            if (!attributeName.StartsWith("@"))
                attributeName = "@" + attributeName;

            collectionOfAttributeExpression.Enqueue(string.Format("not(matches({0},{1}))", attributeName, regexExpression.Contains("'") ? string.Format("\"{0}\"", regexExpression) : string.Format("'{0}'", regexExpression)));
            return this;
        }

        /// <summary>
        /// Creates an expression that validates any attribute that have the expecified attribute value
        /// </summary>
        /// <param name="attributeValue">The value to look for inside attributes</param>
        public XPathExpression WhereAny(string attributeValue)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("@*={0}", attributeValue.Contains("'") ? string.Format("\"{0}\"", attributeValue) : string.Format("'{0}'", attributeValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression that validates any attribute that do not have the expecified attribute value
        /// </summary>
        /// <param name="attributeValue">The value to look for inside attributes</param>
        public XPathExpression WhereNotAny(string attributeValue)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("not(@*={0})", attributeValue.Contains("'") ? string.Format("\"{0}\"", attributeValue) : string.Format("'{0}'", attributeValue)));
            return this;
        }

        /// <summary>
        /// Creates an expression to validate that the length of the inner string of the current tag is equal to the expecified one
        /// </summary>
        /// <param name="length"></param>
        public XPathExpression WhereTextLength(int length)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("string-length() = {0}", length));
            return this;
        }

        /// <summary>
        /// Creates an expression to validate that the length of the inner string of the current tag is not equal to the expecified one
        /// </summary>
        /// <param name="length"></param>
        public XPathExpression WhereNotTextLength(int length)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("not(string-length() = {0})", length));
            return this;
        }

        /// <summary>
        /// Creates an expression to validate that the length of the inner string of the current tag is greater than the expecified one
        /// </summary>
        /// <param name="length"></param>
        public XPathExpression WhereTextLengthGreaterThan(int length)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("string-length() > {0}", length));
            return this;
        }

        /// <summary>
        /// Creates an expression to validate that the length of the inner string of the current tag is greater or equal than the expecified one
        /// </summary>
        /// <param name="length"></param>
        public XPathExpression WhereTextLengthGreaterOrEqualThan(int length)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("not(string-length() >= {0})", length));
            return this;
        }

        /// <summary>
        /// Creates an expression to validate that the length of the inner string of the current tag is lower than the expecified one
        /// </summary>
        /// <param name="length"></param>
        public XPathExpression WhereTextLengthLowerThan(int length)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("string-length() < {0}", length));
            return this;
        }

        /// <summary>
        /// Creates an expression to validate that the length of the inner string of the current tag is lower or equal than the expecified one
        /// </summary>
        /// <param name="length"></param>
        public XPathExpression WhereTextLengthLowerOrEqualThan(int length)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("string-length() <= {0}", length));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element that match with the expecified position
        /// </summary>
        /// <param name="position">The position of element</param>
        public XPathExpression WherePosition(int position)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("position() = {0}", position));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element that do not match with the expecified position 
        /// </summary>
        /// <param name="position">The position of element</param>
        public XPathExpression WhereNotPosition(int position)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("not(position() = {0})", position));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element that is greater than the expecified position 
        /// </summary>
        /// <param name="position">The position of element</param>
        public XPathExpression WherePositionGreaterThan(int position)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("position() > {0}", position));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element that is greater or equal than the expecified position 
        /// </summary>
        /// <param name="position">The position of element</param>
        public XPathExpression WherePositionGreaterOrEqualThan(int position)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("position() >= {0}", position));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element that is lower than the expecified position 
        /// </summary>
        /// <param name="position">The position of element</param>
        public XPathExpression WherePositionLowerThan(int position)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("position() < {0}", position));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element that is lower or equal than the expecified position 
        /// </summary>
        /// <param name="position">The position of element</param>
        public XPathExpression WherePositionLowerOrEqualThan(int position)
        {
            AddDefaultOperator();
            collectionOfAttributeExpression.Enqueue(string.Format("position() <= {0}", position));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for the existence of a parent of an expecified tag
        /// </summary>
        /// <param name="parentTagName">Parent tagname to look for</param>
        /// <param name="applyAsAttribute">If is true it will apply the expression inside []</param>
        public XPathExpression WhereParent(string parentTagName, bool applyAsAttribute = true)
        {
            if (applyAsAttribute)
            {
                AddDefaultOperator();
                collectionOfAttributeExpression.Enqueue(string.Format("./parent::{0}", parentTagName));
            }
            else
                collectionOfPartialExpression.Enqueue(string.Format("parent::{0}", parentTagName));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for the existence of a parent of an expecified tag
        /// </summary>
        /// <param name="parentTagName">Parent expression to apply</param>
        /// <param name="applyAsAttribute">If is true it will apply the expression inside []</param>
        public XPathExpression WhereParent(XPathExpression parentExpression, bool applyAsAttribute = true)
        {
            if (applyAsAttribute)
            {
                AddDefaultOperator();
                collectionOfAttributeExpression.Enqueue(string.Format("./parent::{0}", parentExpression.GetPartialExpression()));
            }
            else
                collectionOfPartialExpression.Enqueue(string.Format("parent::{0}", parentExpression.GetPartialExpression()));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for the existence of a parent, grandparent etc  of an expecified tag
        /// </summary>
        /// <param name="ancestorTagName">Ancestor tagname to look for</param>
        /// <param name="applyAsAttribute">If is true it will apply the expression inside []</param>
        public XPathExpression WhereAncestor(string ancestorTagName, bool applyAsAttribute = true)
        {

            if (applyAsAttribute)
            {
                AddDefaultOperator();
                collectionOfAttributeExpression.Enqueue(string.Format("./ancestor::{0}", ancestorTagName));
            }
            else
                collectionOfPartialExpression.Enqueue(string.Format("ancestor::{0}", ancestorTagName));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for the existence of a parent, grandparent etc of an expecified tag
        /// </summary>
        /// <param name="ancestorExpression">Ancestor expression to apply</param>
        /// <param name="applyAsAttribute">If is true it will apply the expression inside []</param>
        public XPathExpression WhereAncestor(XPathExpression ancestorExpression, bool applyAsAttribute = true)
        {
            if (applyAsAttribute)
            {
                AddDefaultOperator();
                collectionOfAttributeExpression.Enqueue(string.Format("./ancestor::{0}", ancestorExpression.GetPartialExpression()));
            }
            else
                collectionOfPartialExpression.Enqueue(string.Format("ancestor::{0}", ancestorExpression.GetPartialExpression()));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for a previous sibling of the current tag element
        /// </summary>
        /// <param name="siblingTagName">The name of the previous sibling tag</param>
        /// <param name="applyAsAttribute">If is true it will apply the expression inside []</param>
        public XPathExpression WherePrecedingSibling(string siblingTagName, bool applyAsAttribute = true)
        {
            if (applyAsAttribute)
            {
                AddDefaultOperator();
                collectionOfAttributeExpression.Enqueue(string.Format("./preceding-sibling::{0}", siblingTagName));
            }
            else
                collectionOfPartialExpression.Enqueue(string.Format("preceding-sibling::{0}", siblingTagName));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for a previous sibling of the current tag element
        /// </summary>
        /// <param name="siblingExpression">Sibling expression to use</param>
        /// <param name="applyAsAttribute">If is true it will apply the expression inside []</param>
        public XPathExpression WherePrecedingSibling(XPathExpression siblingExpression, bool applyAsAttribute = true)
        {
            if (applyAsAttribute)
            {
                AddDefaultOperator();
                collectionOfAttributeExpression.Enqueue(string.Format("./preceding-sibling::{0}", siblingExpression.GetPartialExpression()));
            }
            else
                collectionOfPartialExpression.Enqueue(string.Format("preceding-sibling::{0}", siblingExpression.GetPartialExpression()));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for a previous sibling of the current tag element
        /// </summary>
        /// <param name="siblingExpression">The name of the next sibling tag</param>
        /// <param name="applyAsAttribute">If is true it will apply the expression inside []</param>
        public XPathExpression WhereFollowingSibling(string siblingTagName, bool applyAsAttribute = true)
        {
            if (applyAsAttribute)
            {
                AddDefaultOperator();
                collectionOfAttributeExpression.Enqueue(string.Format("./following-sibling::{0}", siblingTagName));
            }
            else
                collectionOfPartialExpression.Enqueue(string.Format("following-sibling::{0}", siblingTagName));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for the next sibling of the current tag element
        /// </summary>
        /// <param name="siblingExpression">Sibling expression to use</param>
        /// <param name="applyAsAttribute">If is true it will apply the expression inside []</param>
        public XPathExpression WhereFollowingSibling(XPathExpression siblingExpression, bool applyAsAttribute = true)
        {
            if (applyAsAttribute)
            {
                AddDefaultOperator();
                collectionOfAttributeExpression.Enqueue(string.Format("./following-sibling::{0}", siblingExpression.GetPartialExpression()));
            }
            else
                collectionOfPartialExpression.Enqueue(string.Format("following-sibling::{0}", siblingExpression.GetPartialExpression()));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for a child or deeper child of the current tag element
        /// </summary>
        /// <param name="desdendantTagName">The name of the child tag</param>
        /// <param name="applyAsAttribute">If is true it will apply the expression inside []</param>
        public XPathExpression WhereDescendant(string desdendantTagName, bool applyAsAttribute = true)
        {
            if (applyAsAttribute)
            {
                AddDefaultOperator();
                collectionOfAttributeExpression.Enqueue(string.Format("./descendant::{0}", desdendantTagName));
            }
            else
                collectionOfPartialExpression.Enqueue(string.Format("descendant::{0}", desdendantTagName));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for a child or deeper child of the current tag element
        /// </summary>
        /// <param name="descendantExpression">The name of the child tag</param>
        /// <param name="applyAsAttribute">If is true it will apply the expression inside []</param>
        public XPathExpression WhereDescendant(XPathExpression descendantExpression, bool applyAsAttribute = true)
        {
            if (applyAsAttribute)
            {
                AddDefaultOperator();
                collectionOfAttributeExpression.Enqueue(string.Format("./descendant::{0}", descendantExpression.GetPartialExpression()));
            }
            else
                collectionOfPartialExpression.Enqueue(string.Format("descendant::{0}", descendantExpression.GetPartialExpression()));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for the first child appearance of the current tag element
        /// </summary>
        /// <param name="childTagName">The name of the child tag</param>
        /// <param name="applyAsAttribute">If is true it will apply the expression inside []</param>
        public XPathExpression WhereChild(string childTagName, bool applyAsAttribute = true)
        {
            if (applyAsAttribute)
            {
                AddDefaultOperator();
                collectionOfAttributeExpression.Enqueue(string.Format("./child::{0}", childTagName));
            }
            else
                collectionOfPartialExpression.Enqueue(string.Format("child::{0}", childTagName));
            return this;
        }

        /// <summary>
        /// Creates an expression to look for the first child appearance of the current tag element
        /// </summary>
        /// <param name="childExpression">The name of the child tag</param>
        /// <param name="applyAsAttribute">If is true it will apply the expression inside []</param>
        public XPathExpression WhereChild(XPathExpression childExpression, bool applyAsAttribute = true)
        {
            if (applyAsAttribute)
            {
                AddDefaultOperator();
                collectionOfAttributeExpression.Enqueue(string.Format("./child::{0}", childExpression.GetPartialExpression()));
            }
            else
                collectionOfPartialExpression.Enqueue(string.Format("child::{0}", childExpression.GetPartialExpression()));
            return this;
        }

        /// <summary>
        /// Joins a new expression at the end of the current one
        /// </summary>
        /// <param name="expression">The new expression to add</param>
        public XPathExpression JoinExpression(XPathExpression expression)
        {
            collectionOfPartialExpression.Enqueue(string.Format("{0}", expression.GetPartialExpression()));
            return this;
        }

        /// <summary>
        /// Joins a new expression as an attribute inside [] with the last attribute expression
        /// </summary>
        /// <param name="expression">The new expression to add</param>
        public XPathExpression JoinAttribute(XPathExpression expression)
        {
            collectionOfAttributeExpression.Enqueue(string.Format("{0}", expression.GetPartialExpression()));
            return this;
        }

        /// <summary>
        /// Adds the and operator to an expression *It is the default if ommitted*
        /// </summary>
        public XPathExpression And()
        {
            if (!HasOperator())
                collectionOfAttributeExpression.Enqueue(operators[0]);
            return this;
        }

        /// <summary>
        /// Adds the or operator to an expression *It if ommitted it will add the and operator*
        /// </summary>
        public XPathExpression Or()
        {
            if (!HasOperator())
                collectionOfAttributeExpression.Enqueue(operators[1]);
            return this;
        }

        /// <summary>
        /// Creates an expression object using the exprecified tag
        /// </summary>
        /// <param name="tagName">The tagname to use</param>
        public static XPathExpression From(string tagName)
        {
            return new XPathExpression(tagName);
        }

        /// <summary>
        /// Creates an expression object using the * to look for any element
        /// </summary>
        public static XPathExpression FromAny()
        {
            return new XPathExpression("*");
        }

        /// <summary>
        /// Creates an expression object without using a tagname *This is usefull to join expressions*
        /// </summary>
        public static XPathExpression FromEmpty()
        {
            return new XPathExpression(string.Empty);
        }

    }
}

