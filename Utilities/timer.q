/*****Natalie Inkpin*****20/01/2013*****
/email: natalie.inkpin@aquaq.co.uk



/aquaq contact details: info@aquaq.co.uk
/                       training.admin@aquaq.co.uk


/For the latest version of the timer.q script, plus more, please visit www.aquaq.co.uk/q_code_contributions

/*****SUMMARY*****
/When the timer script is executed, it allows the user to define either a single occurrence of a timer or a repeating timer. The script can be loaded in by
/performing \l timer.q and the timer namespace can be accessed by \d .timer. The timer table holds all the information about the created timers, based on the
/parameter inputs given by the user in the function calls. This provides a useful backtrack of the timer calls that have/have to be performed. The table is 
/constantly upserted when new tables are created. Typing .timer.timer when the timer namespace has been loaded shows the table. 
/The main functions that can be used to create a timer are: .timer.repeat and .timer.once. .timer.repeat makes a call on the defined rep function which updates 
/the timer table based on the parameter info supplied. The parameters used in this repeating timer call are:
/start - timestamp of what date and time the timer starts                  end - timestamp of what date and time the timer ends
/period - timespan of how often the timer runs                             funcparam - input predefined function and parameter to use in the timer
/nextsch - determines how the timer is scheduled next                      descrip - timer description
/dupcheck - set to true if duplicated timers are not allowed                
/                                                                          Use: repeat[start;end;period;funcparam;nextsch;descrip;dupcheck]
/The nextschedule variable must be either 0 1 2h as the depending on the number defined, it determines how the repeating timer is scheduled. Assuming that a timer function with period P
/is scheduled to fire at T0, actually fires at T1 and ends at T2, then mode 0 is P+T0, mode 1 is P+T1, mode 2 is P+T2.
/Therefore if 0h then period and nextrun are added together to determine the next time. If the value is 1h then period and the start time are added, and if the value is 2h then period and the current timestamp are added.
/.timer.once makes a call on the defined one function which updates the timer table based on the parameter info supplied for the one-off timer.
/The parameters used in this repeating timer call are:
/runtime - the timestamp of when the timer should end                      funcparam - input predefined function and parameter to use in the timer
/descrip - timer description                                               dupcheck - set to true if duplicated timers are not allowed 

/                                                                           Use: one[runtime;funcparam;descrip;dupcheck] 
/There are also remove functions, .timer.remove and .timer.removefunc, which can be used to removed a whole timerID from the table or only a funcparam, by
/supplying the timerID to the function call. Various verification checks are applied to the function calls such as dupe checks, type checks and valid value checks.
/If debug mode is set, this can explain how timer calls could possibly go wrong and how to fix the execution errors.


\d .timer
/***********
/changes the namespace to q.timer in order for variables to be defined under the timer namespace
/***********


debug:@[value;`debug;0b]				
logcall:@[value;`logcall;1b]				
nextscheduledefault:@[value;`nextscheduledefault;2h]
/*************
/protected execution - if the values of debug, logcall or nextscheduledefault are not yet set, then set 0b, 1b and 2h respectively for each variable
/*************


id:0
getID:{:id+::1}
/*************
/when the getID function is called then the value of id increases by one each time, it amends the id variable
/*************


timer:([id:`int$()]			/- the id of the timer
 timerchange:`timestamp$();		/- when the function was added to the timer
 periodstart:`timestamp$();		/- the first time to fire the timer 
 periodend:`timestamp$(); 		/- the the last time to fire the timer	
 period:`timespan$();			/- how often the timer is run
 funcparam:();				/- the function and parameters to run
 lastrun:`timestamp$();			/- the last run time
 nextrun:`timestamp$();			/- the next scheduled run time
 active:`boolean$();			/- whether the timer is active
 nextschedule:`short$();		/- determines how the next schedule time should be calculated
 description:());			/- a free text description
 /**************
 /setting the foundations for the timer table which will keep all information about the timer setup, all column types are set to a time related type
 /apart from active, description and funcparam
 /**************
 
  
 check:{[fp;dupcheck]
	if[dupcheck;
		if[count select from timer where fp~/:funcparam;
                        '"duplicate timer already exists for function ",(-3!fp),". Use .timer.rep or .timer.one with dupcheck set to false to force the value"]];
	$[0=count fp; '"funcparam must not be an empty list";
	  10h=type fp; '"funcparam must not be string.  Use (value;\"stringvalue\") instead";
	  fp]}
 /***************
 /if dupcheck is of value 1, a second if statement is then performed which is a duplicate timer check. 
 /This tests what is supplied for fp against each funcparam in the table (each right). If the count is 1 that means that a timer already exists and a warning output is displayed
 /-3!fp returns the string representation of fp. If the variable fp contains no input then a warning statement is shown stating it must not be an empty list
 /If the type of the variable fp is a string (10h) then a warning statement is shown. If neither of these if conditions occur then fp is returned
 /****************
 
 
/- add a repeatingtimer
rep:{[start;end;period;funcparam;nextsch;descrip;dupcheck] 
	if[not nextsch in `short$til 3; '"nextsch mode can only be one of ",-3!`short$til 3];
	`.timer.timer upsert (getID[];.z.p;start:.z.p^start;0Wp^end;period;check[funcparam;dupcheck];0Np;start+period;1b;nextsch;descrip);}
/******************
/If the variable nextsch is not of a short typed number within 0 1 2h then the warning statement is outputted.
/The new timer information is upserted to the table. The timerID is increased by one due to the getID function
/The gmt timestamp is added to timerchange, the start variable can be any timestamp in the future and this is filled into the current .z.p 
/0Wp is the infitive timestamp and the end timestamp is filled to it and appended to period end. The check function is performed for funcparam where if passed then the fp variable is outputted. 
/Nextrun is the sum of the starting timestamp and the period of how often the timer runs. 
/******************


/- add a one off timer
one:{[runtime;funcparam;descrip;dupcheck] 
        `.timer.timer upsert (getID[];.z.p;.z.p;0Np;0Nn;check[funcparam;dupcheck];0Np;runtime;1b;0h;descrip);}
/********************
/the timer table is upserted to with:
/the ID being increased by one, the timerchange and the start is set to the current gmt timestamp, timer end is set to null timestamp, period is set to null timespan. 
/No last run has been performed so this is a null timestamp. Nextschedule is set to default 0h.
/********************


/- projection to add a default repeating timer.  Scheduling mode 2 is the safest - least likely to back up
repeat:rep[;;;;nextscheduledefault;;1b]
once:one[;;;1b]
/********************
/These will be the functions used to setup the timers
/********************


/- Remove a row from the timer 
remove:{[timerid] delete from `.timer.timer where id=timerid}
removefunc:{[fp] delete from `.timer.timer where fp~/:funcparam}
/********************
/To remove an entire row from the timer table, calling the remove function with a specific timer id will delete it
/If a function parameter needs deleting from the timer table then if fp is matched to any of the funcparams then it is removed
/********************


/- run a timer function and reschedule if required
run:{
	/- Pull out the rows to fire
	/- Assume we only use period start/end when creating the next run time
	/- sort asc by lastrun so the timers which are due and were fired longest ago are given priority
	torun:`lastrun xasc 0!select from timer where active,nextrun<x;	
	runandreschedule each torun}
/********************	
/Select the timer id in the table that has a nextrun less than the timestamp supplied.
/The 0! is to convert a keyed table to a standard primitive non-keyed table where the extracted rows are sorted by the lastrun column and the timer waiting 
/the longest is then ran. The runandreschedule method below is applied to each extracted row
/********************
 
  
 /- run a timer function and reschedule it if required
runandreschedule:{
	/- if debug mode, print out what we are doing 	
	if[debug; -1"running timer ID ",(string x`id),". Function is ",-3!x`funcparam];
	start:.z.p;
	@[$[logcall;0;value];x`funcparam;{update active:0b from `.timer.timer where id=x`id; -2"timer ID ",(string x`id)," failed with error ",y,".  The function will not be rescheduled"}[x]];
	/- work out the next run time
	n:x[`period]+(x[`nextrun];start;.z.p) x`nextschedule;
	/- check if the next run time falls within the sceduled period
	/- either up the nextrun info, or switch off the timer
	$[n within x`periodstart`periodend;
		update lastrun:start,nextrun:n from `.timer.timer where id=x`id;
		[if[debug;-1"setting timer ID ",(string x`id)," to inactive as next schedule time is outside of scheduled period"];
		 update lastrun:start,active:0b from `.timer.timer where id=x`id]];
	}
/*******************
/if debug is set to true then print out which timer is running and the funcparams. The particular columns to be stringed are pulled out by string x`id.
/Protected execution is implemented in order to check whether the timer can run without error, if not then the timer id and the error in the log is outputted.
/If logcall is true then 0 will return as the first parameter otherwise value will return. The second parameter is extracting the funcparams, this only performs if the first parameter on x is performed. 
/Within n, to each period timespan row it adds either the nextrun timestamp to it, the start timestamp or the current timestamp depending on what value nextschedule is inputted.

/An if else is performed where if n is within the first timer start and the last timer start, then the next statement is executed where the lastrun is updated as 
/the start time of the timer, and the nextrun column is updated to the timestamp n.
/When n is not within periodstart and end, assuming debug mode is true then a warning output is shown explaining that the next run time is outside the limits.
/The timer will not be performed, lastrun will be the start timestamp and active is set to false for this timer ID.
/*******************


loaded:1b

/- Set .z.ts
$[@[{value x;1b};`.z.ts;0b];
	.z.ts:{.timer.run[y]; x@y}[.z.ts];
	.z.ts:{.timer.run[x]}];

/- Set the timer to 200ms if not set already
if[not system"t"; system"t 200"]
/********************
/an if-else statement is performed where the first parameter applies protected execution where if .z.ts has been set to be called at certain time intervals with output
/then 1b is returned, however if the value operation cannot be performed on .z.ts (not set yet) then 0b is returned.

/If the protected execution returns true then .z.ts is set to be the function stated in the second parameter, where the set .z.ts is passed into the new definition
/of .z.ts. If false however, .z.s is set to be the function stated in the last parameter. 

/In both cases the run function within the timer namespace is executed with parameter passed in (either x or y).
/In the second parameter the x@y means that as the set .z.ts is passed through, this would be recognised as the variable x. The @ means that this function will be 
/applied to the y variable

/If there is not yet a timer value t established then the if statement will set t to be 200ms. The system"t" command is the same as \t
/********************

\
f:{0N!`firing;x+1}
f1:{0N!`firing;system"sleep ",string x}
repeat[.z.p;.z.p+0D00:01;0D00:00:15;(f1;2);"test timer"]
rep[.z.p;.z.p+0D00:01;0D00:00:15;(f1;3);0h;"test timer";1b]
rep[.z.p;.z.p+0D00:01;0D00:00:15;(f1;4);1h;"test timer";1b]

once[.z.p+0D00:00:10;(`.timer.f;2);"test once"]  
.z.ts:run
\t 500
/**********************
/The function f displays the timer is firing with the sum result of x+1
/The function f1 outputs the symbol firing and performs the unix command sleep meaning that the execution will sleep for x amount of seconds
/The repeat, rep and once functions are now being executed with parameters. .z.ts is set to be the function run and timer is set to display output every 500 seconds
/**********************

/
/*****MORE EXAMPLES*****
/1) removing an existing timer id
/remove[1]
/`.timer.timer
/q.timer)timer
/id| timerchange periodstart periodend period funcparam lastrun nextrun active..
/--| -------------------------------------------------------------------------..


/2) altering the repeating timer setup so it stops after two minutes and performs every 5.005 seconds, with an output of timer run. The timer table is also shown with the timer info
/q.timer)f1:{0N!`timerrun;system"sleep ",string x}
/q.timer)rep[.z.p;.z.p+0D00:02;0D00:00:05.005;(f1;1);1h;"timer run";1b]
/q.timer)`timerrun
/`timerrun
/`timerrun
/timer
/id| timerchange                   periodstart                   periodend    ..
/--| -------------------------------------------------------------------------..
/2 | 2014.01.14D18:03:40.847700000 2014.01.14D18:03:40.847678000 2014.01.14D18..
/q.timer)`timerrun
/`timerrun
/`timerrun
/`timerrun
/`timerrun


/3) performing a timer once with debug mode on
/q.timer)f:{0N!`firing;x*2}
/q.timer)once[.z.p+0D00:00:05;(`.timer.f;2);"test once"]
/q.timer)running timer ID 10. Function is (`.timer.f;2)
/`firing
/setting timer ID 10 to inactive as next schedule time is outside of scheduled period


/4) Another example of a timer running every 30 mins and ending in a day's time
/repeat[.z.p;.z.p+1D05:05;0D00:30:00;(f1;6);"timer ran successfully: exiting timer"]


/5) Example of the nextsch variable being out of bounds
/rep[.z.p;.z.p+0D00:02;0D00:00:05.005;(f1;1);4h;"timer run";1b]
/'nextsch mode can only be one of 0 1 2h


/6) What will have if a duplicate timer tries to execute when duplicate check is set to true
/q.timer))rep[.z.p;.z.p+0D00:02;0D00:00:05.005;(f1;1);1h;"timer run";1b]
/{[start;end;period;funcparam;nextsch;descrip;dupcheck]
 /if[not nextsch in `short$til 3; '"nextsch mode can only be one of ",-3!`short$til 3];
 /`.timer.timer upsert (getID[];.z.p;start:.z.p^start;0Wp^end;period;check[funcparam;dupcheck];0Np;start+period;1b;nextsch;descrip);}
/'duplicate timer already exists for function ({0N!`timerrun;system"sleep ",string x};1). Use .timer.rep or .timer.one with dupcheck set to false to force the value


/7) What happens when debug mode is on
/q.timer)f1:{0N!`timerrun;system"sleep ",string x}
/q.timer)debug:1b
/q.timer)rep[.z.p;.z.p+0D00:00:10;0D00:00:02;(f1;2);1h;"timer run";1b]
/q.timer)running timer ID 1. Function is ({0N!`timerrun;system"sleep ",string x};2)
/`timerrun
/running timer ID 1. Function is ({0N!`timerrun;system"sleep ",string x};2)
/`timerrun
/running timer ID 1. Function is ({0N!`timerrun;system"sleep ",string x};2)
/`timerrun
/running timer ID 1. Function is ({0N!`timerrun;system"sleep ",string x};2)
/`timerrun
/setting timer ID 1 to inactive as next schedule time is outside of scheduled period


/8) The function parameter is not of suitable type, should be a list
/rep[.z.p;.z.p+0D00:00:10;0D00:00:02;1;2h;"timer run";0b]
/q.timer)running timer ID 3. Function is 1
/timer ID 3 failed with error length.  The function will not be rescheduled


/9)Removing a function parameter from the timer table
/q.timer)select funcparam from timer
/funcparam
/------------------------------------------
/({0N!`timerrun;system"sleep ",string x};2)
/enlist(;2)
/1
/1
/`1
/`1
/
/q.timer)removefunc[1]
/`.timer.timer
/q.timer)select funcparam from timer
/funcparam
/------------------------------------------
/({0N!`timerrun;system"sleep ",string x};2)
/enlist(;2)
/`1
/`1


/10) Setting the start of the timer to be later than the end time
/q.timer)debug:1b
/q.timer)rep[.z.p+00:00:10;.z.p;0D00:00:03;(f1;1);2h;"timer run";1b]
/q.timer)running timer ID 9. Function is ({0N!`timerrun;system"sleep ",string x};1)
/`timerrun
/setting timer ID 9 to inactive as next schedule time is outside of scheduled period