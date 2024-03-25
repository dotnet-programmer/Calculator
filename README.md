# Calculator 

Calculator written with C#, WPF, XAML, MVVM.

## Methods for calculating an expression
- Expression parser
- Infix to postfix
- DataTable

### Expression parser

Grammar used to handle expressions:

	Expression:
        Term
        Expression "+" Term
        Expression "-" Term

    Term:
        Primary
        Term "*" Primary
        Term "/" Primary
        Term "%" Primary

    Primary:
        Number
        Name
        "(" Expression ")"
        "+" Primary
        "-" Primary
        "pow(" Primary "," Primary ")"
        "sqrt(" Primary ")"

    Number:
        floating-point-literal

### Infix to postfix

Algorithm: https://www.geeksforgeeks.org/convert-infix-expression-to-postfix-expression/

### DataTable

Using the Compute method from the class System.Data.DataTable.
