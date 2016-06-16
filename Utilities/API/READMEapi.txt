2013/12/05
Tobias Harper
tobias.harper@aquaq.co.uk
AQUAQ Analytics
Info@aquaq.co.uk 
+4402890511232

API.Q
More help on this, and other subjects, is available in help.q. 
Find the help file at: http://www.kx.com/q/d/help.q

The api.q file is a short script used to define some functions useful in building up an application programming interface. 
The purpose of this script is to provide a platform that allows interface variables and functions to be stored in a descriptive API table across all namespaces, ensuring an up to date record of general use functions that can be added to at any point. 
Each API entry is to contain information on a variable name, public flag, general description, required parameters, and what the function should return. 
The code allows “pattern matching” searching of memory for any variables or functions consistent with the input, and returns all matching API entries in the API table. 
The input can be single values or part of a specific string, and searches can be conducted in general, restricted to public functions, or user defined to exclude standard namespaces (.q, .Q, .h, .o). 



Download the api.q script into the home directory:
q) \l api.q


Basic functions:
add:[name;public;descrip;params;return]
The user should use the add function to insert descriptive definitions of functions, along with their required parameters and what should be returned, into the API table. Only function definitions that the user wishes to be searchable (i.e. usable by others) should be entered. 

.api.detail
All manually entered entries can be viewed in the details tables: .api.detail.

.api.f, .api.p, .api.s 
The .api.f and .api.p functions can be used to find normal and public values respectively, whilst the .api.s function searches for patterns in the function definition. More refined searches can be carried out using the .api.find and .api.search functions, which allow context sensitivity to be applied to the search. The special function .api.u carries out public searches excluding .q, .Q, .h, and .o namespaces.

Further functions enable the retrieval of full variable names and full paths, as well as listing of a given namespace for a specific variable type.

Examples
show .api.f`ad			/- search for a value
show .api.p`ad			/- search for a public value
show .api.p"*ad"		/- search for a public specific pattern
show .api.f"*ad"		/- search for a specific pattern
show .api.u`ad			/- search for a public value, exclude standard namespaces (.q, .Q, .h, .o)
show .api.u"*ad"		/- Search for a public specific pattern, exclude standard namespaces.
show .api.s["*api*"]		/- search the function definitions for the supplied pattern
show .api.search[“*ad*”; 1b]	/- search the definition of functions for a supplied pattern, with context sensitive flag
show .api.find[`ad;1b;1b]	/- search for a value, with public and context sensitive flag.
show .api.m[]			/- show the memory usage evaluation of each variable in the process, including all views.
show .api.mem[0b]		/- show the memory usage evaluation of each variable in the process, not including views.
show .api.fullapi[]		/- show the full function API table.



