Feature: Purchase Fuel
	Test the ensek API

Background: 
	Given I log on to the system under test
	And I reset the fuel data
	And I retrieve a report of current energy levels

Scenario: Purchase electricity
	When I purchase 3 units of fuel 3
	Then the reported purchase quantity is 3
	And the reported remaining quantity is 3 less than the orginal total available
	And the reported price matches the orginal price multiplied by 3
	And the number of orders made before today is 5

Scenario: Purchase gas
	When I purchase 10 units of fuel 1
	Then the reported purchase quantity is 10
	And the reported remaining quantity is 10 less than the orginal total available
	And the reported price matches the orginal price multiplied by 10
    And the number of orders made before today is 5

Scenario: Purchase oil
	When I purchase 2 units of fuel 4
	Then the reported purchase quantity is 2
	And the reported remaining quantity is 2 less than the orginal total available
	And the reported price matches the orginal price multiplied by 2
	And the number of orders made before today is 5

Scenario: Purchase nuclear
	When I purchase 1 units of fuel 2
	Then the response message is "There is no nuclear fuel to purchase!"
	And the number of orders made before today is 5
