# InvestCalc
Program to keep track of investments and calculate their returns.
Desktop and mobile apps sharing same back-end source code (.NET and Xamarin).

![Screenshot](https://i.postimg.cc/zGWbYDp2/Invest-Calc.png "Example of main window, on Windows environment")

![Screenshot](https://i.postimg.cc/9fwXdB82/Invest-Calc-0.png "Example of main screen, on Android environment")
![Screenshot](https://i.postimg.cc/tThXf4Gm/Invest-Calc-1.png "Stock menu, on Android environment")
![Screenshot](https://i.postimg.cc/m2rs3wkn/Invest-Calc-2.png "Main options, on Android environment")
![Screenshot](https://i.postimg.cc/Cx3pgp7y/Invest-Calc-3.png "History of buy/sell/dividend/cost operations, on Android environment")

## Features
* Register orders
(buy/sell shares, dividends/costs).

* Persistent disk database
(portfolio and order data persist between sessions).

* Display the current portfolio
(currently owned shares of every stock).

* Retrieve stock prices from the Internet
(via <https://www.alphavantage.co>
-- license key needed, freely available).
For this the "fetchCode" of each stock must be entered into the "stock data".
An online search function for these codes is also included in the app.
Prices can also be entered manually.

* Display and manipulate order history.
Can filter by dates and stocks.
Can be imported and exported from/to CSV.

* Calculate the equivalent yearly return of each investment,
or all or several investments together.
The equivalent yearly return is defined as the interest/discount rate
that makes the net present value of all cash flows
equal to the current value
(current price multiplied by number of shares currently owned).

## Dependencies
* [libcs-math](https://github.com/XavierAP/libcs-math)
is used to calculate the returns,
by Newton-Raphson's iterative method.

* [libcs-sqlite](https://github.com/XavierAP/libcs-sqlite)
is used for SQLite database implementation.
Note: the desktop app requires the 'master' branch of this library
for its extra binding features,
whereas the Xamarin apps require the 'microsoft' branch
(will otherwise throw at run-time, at least with the current System.Data.SQLite version).
Not my fault... :(

* [libcs-utils](https://github.com/XavierAP/libcs-utils)
