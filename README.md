# Introduction
This project contains the source code for the Speedhive C# assignment.

# Building and running
```bash
dotnet run --project src/App
```

# Speedhive C# Assignment 
MYLAPS Sports Timing offers the best-in-class sports timing systems to measure, publish and analyze race and practice results for all sports. In this assignment we ask you create an API that demonstrates how you build resilient, testable code. It does not have to be a fully functioning, polished solution. 
The solution should be build using C# on a recent .net version.

The API should expose a service that calculates the result of a race based on lap times collected during a race. Next to the result, the API should also include weather information.

## Requirements
1.	A race consists of up to five drivers racing around the track for a fixed number of laps (driving from the start/finish line to the start/finish line). 
2.	The driver number and the time of day are measured by the timing system each time a driver passes the start/finish line of the racing track. The API should accept these lap times in the form of the attached karttimes.csv file. 
3.	The race is finished when one of the drivers completes all laps. 
4.	The winner of a race is the driver who has driven the fastest lap. 
5.	The API should return the winner of the race, including the winning lap.
6.	The API should include weather information fetched from a 3rd party api. Any weather service is good (we’d like to see how you treat external resources). You can find some free ones here: link.

## Notes
*	Your assignment will be evaluated on technical design, code quality and commit hygiene.
*	Make sure that your project is accessible on a git repository.
*	Please do not spend more than 8 hours on this assignment.

# TODO
* Add the external weather API