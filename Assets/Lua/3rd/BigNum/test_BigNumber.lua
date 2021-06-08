---
---                 ColaHwLua
--- Copyright © 2018-2049 ColaHwLua Moha
---              test_BigNumber Module业务逻辑
---

require("3rd.BigNum.BigNumber")

local a = BigNumber.new("19999,0");
local b = BigNumber.new(20000);
local c = BigNumber.new(200,2);
Log.debug("a=="..json.encode(a)..";b=="..json.encode(b)..";c=="..json.encode(c))
local d = -1;
if a== b then
	d = "true"
else
	d = "false"
end		
Log.debug("a==b"..d )
if a<b then
	d = "true"
else
	d = "false"
end
Log.debug("a<b"..d )
if a<=b then
	d = "true"
else
	d = "false"
end
Log.debug("a<=b"..d )
if a>b then
	d = "true"
else
	d = "false"
end
Log.debug("a>b"..d )
if a>=b then
	d = "true"
else
	d = "false"
end
Log.debug("a>=b"..d )
b= 1000;
d = a +b;
Log.debug("a+b"..BigNumber.mt.tostring(d) )
d = a -b;
Log.debug("a-b"..BigNumber.mt.tostring(d) )
d = a *b;
Log.debug("a*b"..BigNumber.mt.tostring(d) )
d = a /b;
Log.debug("a/b"..BigNumber.mt.tostring(d) )