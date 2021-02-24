# InvestCalc
Program to keep track of investments and calculate their returns.
Desktop version complete, mobile apps in progress.
Both share same back-end source code (.NET and Xamarin).

![Screenshot](https://i.postimg.cc/zGWbYDp2/Invest-Calc.png "Example of main window")

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

* [libcs-utils](https://github.com/XavierAP/libcs-utils)
