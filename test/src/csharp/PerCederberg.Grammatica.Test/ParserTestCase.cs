/*
 * ParserTestCase.cs
 *
 * This work is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation; either version 2 of the License,
 * or (at your option) any later version.
 *
 * This work is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
 * USA
 *
 * As a special exception, the copyright holders of this library give
 * you permission to link this library with independent modules to
 * produce an executable, regardless of the license terms of these
 * independent modules, and to copy and distribute the resulting
 * executable under terms of your choice, provided that you also meet,
 * for each linked independent module, the terms and conditions of the
 * license of that module. An independent module is a module which is
 * not derived from or based on this library. If you modify this
 * library, you may extend this exception to your version of the
 * library, but you are not obligated to do so. If you do not wish to
 * do so, delete this exception statement from your version.
 *
 * Copyright (c) 2003 Per Cederberg. All rights reserved.
 */

using System;
using System.IO;

using PerCederberg.Grammatica.Parser;

namespace PerCederberg.Grammatica.Test {

    /**
     * Base class for all the parser test cases.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  1.0
     */
    public abstract class ParserTestCase {
    
        /**
         * Parses with the parser and checks the output. If the parsing
         * failed or if the tree didn't match the specified output, a test
         * failure will be reported. 
         * 
         * @param parser         the parser to use
         * @param output         the expected parse tree
         */
        protected void Parse(Parser parser, string output) {
            try {
                ValidateTree(parser.Parse(), output);
            } catch (ParserCreationException e) {
                Fail(e.Message);
            } catch (ParserLogException e) {
                Fail(e.GetError(0).GetMessage());
            }
        }
        
        /**
         * Parses with the parser and checks the parse error. If the 
         * parsing succeeded or if the parse exception didn't match the 
         * specified values, a test failure will be reported. 
         * 
         * @param parser         the parser to use
         * @param type           the parse error type
         * @param line           the line number
         * @param column         the column number
         */
        protected void FailParse(Parser parser, 
                                 ParseException.ErrorType type, 
                                 int line, 
                                 int column) {
    
            try {
                parser.Parse();
                Fail("parsing succeeded");
            } catch (ParserCreationException e) {
                Fail(e.Message);
            } catch (ParserLogException e) {
                ParseException  p = e.GetError(0);  
            
                AssertEquals("error count", 1, e.GetErrorCount());
                AssertEquals("error type", type, p.GetErrorType());
                AssertEquals("line number", line, p.GetLine());
                AssertEquals("column number", column, p.GetColumn());
            }
        }
    
        /**
         * Validates that a parse tree is identical to a string 
         * representation. If the two representations mismatch, a test 
         * failure will be reported.
         * 
         * @param root           the parse tree root node
         * @param str            the string representation
         */
        private void ValidateTree(Node root, string str) {
            StringWriter output = new StringWriter();
            
            root.PrintTo(output);
            ValidateLines(str, output.ToString());
        }
        
        /**
         * Validates that two strings are identical. If the two strings 
         * mismatch, a test failure will be reported.
         * 
         * @param expected       the expected result
         * @param result         the result obtained
         */
        private void ValidateLines(string expected, string result) {
            int  line = 1;
            int  pos;
    
            pos = result.IndexOf('\n');
            while (pos > 0) {
                if (expected.Length < pos) {
                    break;
                }
                AssertEquals("on line: " + line,
                             expected.Substring(0, pos), 
                             result.Substring(0, pos));
                expected = expected.Substring(pos + 1);
                result = result.Substring(pos + 1);
                pos = result.IndexOf('\n');
                line++;
            }
            AssertEquals("on line: " + line, expected, result);
        }
        
        /**
         * Throws a test fail exception.
         *
         * @param message         the test error message
         */
        protected void Fail(string message) {
            throw new Exception(message);
        }
        
        /**
         * Checks that two values are identical. If the values are not
         * identical, a test failure will be reported.
         *
         * @param label          the error label
         * @param expected       the expected value
         * @param result         the obtained value
         */
        protected void AssertEquals(string label, 
                                    object expected, 
                                    object result) {

            if (!expected.Equals(result)) {
                Fail(label + ", expected: " + expected +
                     ", found: " + result);
            }
        }
    }
}