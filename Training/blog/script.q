// Load JSON.library - http://kx.com/q/e/json.k
\l json.k

dataformat:{`name`data!(x;y)};
evaluate:{dataformat[x`func;(value x`func) @ value x _ `func]};
.z.ws:{neg[.z.w] -8!.j.j @[evaluate;.j.k -9!x;{'"error: ",x}]};
