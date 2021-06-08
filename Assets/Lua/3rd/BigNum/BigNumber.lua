--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%{{{1
--
--  File Name:              bignum.lua
--  Package Name:           BigNum
--
--  Project:    Big Numbers library for Lua
--  Mantainers: fmp - Frederico Macedo Pessoa
--              msm - Marco Serpa Molinaro
--
--  History:
--     Version      Autor       Date            Notes
--      1.1      fmp/msm    12/11/2004   Some bug fixes (thanks Isaac Gouy)
--      alfa     fmp/msm    03/22/2003   Start of Development
--      beta     fmp/msm    07/11/2003   Release
--
--  Description:
--    Big numbers manipulation library for Lua.
--    A Big Number is a table with as many numbers as necessary to represent
--       its value in base 'RADIX'. It has a field 'len' containing the num-
--       ber of such numbers and a field 'signal' that may assume the values
--       '+' and '-'.
--
--$.%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


--%%%%%%%%  Constants used in the file %%%%%%%%--{{{1
DEG = 10;
LEN = 7 ;


--%%%%%%%%        Start of Code        %%%%%%%%--

BigNumber = {} ;
BigNumber.mt = {} ;


--BigNum.new{{{1
--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
--
--  Function: New
--
--
--  Description:
--     Creates a new Big Number based on the parameter num.
--
--  Parameters:
--     num - a string, number or BigNumber.
--
--  Returns:
--     A Big Number, or a nil value if an error occured.
--
--
--  %%%%%%%% --

function BigNumber.new( num1,num2 ) --{{{2
	local bignumber = {x=0,y=0} ;
	setmetatable( bignumber , BigNumber.mt ) ;
	if num2 == nil then
		if type(num1) == "table" then
			if num1.x == nil then
				error("num1 结构不对")
				return nil
			else
				if num1.x == 0 or num1.x == "0" then
					bignumber.x = 0;
					bignumber.y = 0;
				else
					--local moreUnit = math.modf(math.log10(math.abs(num1.x)))-LEN;
					--if moreUnit <0 then
						--bignumber.x = 0;
						--bignumber.y = num1.y;
					--else
						--bignumber.x = num1.x / math.pow(DEG,moreUnit);
						--bignumber.y = num1.y + moreUnit; 
					--end	
					bignumber = BigNumber.convert(num1);
				end	
				return bignumber;
			end
		elseif type(num1) == "string" then
			local arr = string.split(num1,",");
			bignumber.x = tonumber(arr[1]);
			bignumber.y = tonumber(arr[2]);
			bignumber = BigNumber.convert(bignumber);
		elseif type(num1) == "number" then
			if num1 == 0 then
				bignumber.x =0;
				bignumber.y =0;
			else
				bignumber.y = math.modf(math.log10(math.abs(num1)))- LEN;
				bignumber.x = num1 / math.pow(DEG,bignumber.y);
			end
			bignumber = BigNumber.convert(bignumber);
			
		else
			error("num1 非指定类型(table、number、string),num1类型为"..type(num1));
			return  nil
		end					
	else
		bignumber.x = tonumber(num1);
		bignumber.y = tonumber(num2);
		bignumber = BigNumber.convert(bignumber);
	end
	return bignumber ;
end

--%%%%%%%%%%%%%%%%%%%% Functions for metatable %%%%%%%%%%%%%%%%%%%%--{{{1
--BigNum.mt.sub{{{2
function BigNumber.mt.sub( num1 , num2 )
	local bnum1 = BigNumber.new(num1);
	local bnum2 = BigNumber.new(num2);
	bnum1 = BigNumber.sub( bnum1 , bnum2 ) ;
	return bnum1 ;
end

--BigNum.mt.add{{{2
function BigNumber.mt.add( num1 , num2 )
	local bnum1 = BigNumber.new(num1);
	local bnum2 = BigNumber.new(num2);
	bnum1 = BigNumber.add( bnum1 , bnum2  ) ;
	return bnum1 ;
end

--BigNum.mt.mul{{{2
function BigNumber.mt.mul( num1 , num2 )
	local bnum1 = BigNumber.new(num1);
	local bnum2 = BigNumber.new(num2);
	bnum1 = BigNumber.mul( bnum1 , bnum2  ) ;
	return bnum1 ;
end

--BigNum.mt.div{{{2
function BigNumber.mt.div( num1 , num2 )
	local bnum1 = BigNumber.new(num1);
	local bnum2 = BigNumber.new(num2);
	bnum1 = BigNumber.div( bnum1 , bnum2 ) ;
	return bnum1 ;
end

--BigNum.mt.tostring{{{2
function BigNumber.mt.tostring( bnum )
	return bnum.x..","..bnum.y;
end



--BigNum.mt.eq{{{2
function BigNumber.mt.eq( num1 , num2 )
	local bnum1 = BigNumber.new(num1);
	local bnum2 = BigNumber.new(num2);
	return BigNumber.eq( bnum1 , bnum2 ) ;
end

--BigNum.mt.lt{{{2
function BigNumber.mt.lt( num1 , num2 )
	local bnum1 = BigNumber.new(num1);
	local bnum2 = BigNumber.new(num2);
	return BigNumber.lt( bnum1 , bnum2 ) ;
end

--BigNum.mt.le{{{2
function BigNumber.mt.le( num1 , num2 )
	local bnum1 = BigNumber.new(num1);
	local bnum2 = BigNumber.new(num2);
	return BigNumber.le( bnum1 , bnum2 ) ;
end

----BigNumber.mt.gt{{{2
--function BigNumber.mt.gt( num1 , num2 )
	--local bnum1 = BigNumber.new(num1);
	--local bnum2 = BigNumber.new(num2);
	--return BigNumber.gt( bnum1 , bnum2 ) ;
--end
----BigNumber.mt.ge{{{2
--function BigNumber.mt.ge( num1 , num2 )
	--local bnum1 = BigNumber.new(num1);
	--local bnum2 = BigNumber.new(num2);
	--return BigNumber.ge( bnum1 , bnum2 ) ;
--end



--%%%%%%%%%%%%%%%%%%%% Metatable Definitions %%%%%%%%%%%%%%%%%%%%--{{{1

BigNumber.mt.__metatable = "hidden"           ; -- answer to getmetatable(aBignum)
-- BigNum.mt.__index     = "inexistent field" ; -- attempt to acess nil valued field
-- BigNum.mt.__newindex  = "not available"    ; -- attempt to create new field
BigNumber.mt.__tostring  = BigNumber.mt.tostring ;
-- arithmetics
BigNumber.mt.__add = BigNumber.mt.add ;
BigNumber.mt.__sub = BigNumber.mt.sub ;
BigNumber.mt.__mul = BigNumber.mt.mul ;
BigNumber.mt.__div = BigNumber.mt.div ;
-- Comparisons
BigNumber.mt.__eq = BigNumber.mt.eq   ;
BigNumber.mt.__le = BigNumber.mt.le   ;
BigNumber.mt.__lt = BigNumber.mt.lt   ;
--BigNumber.mt.__ge = BigNumber.mt.ge   ;
--BigNumber.mt.__gt = BigNumber.mt.gt   ;
--concatenation
-- BigNum.me.__concat = ???

setmetatable( BigNumber.mt, { __index = "inexistent field", __newindex = "not available", __metatable="hidden" } ) ;

--%%%%%%%%%%%%%%%%%%%% Basic Functions %%%%%%%%%%%%%%%%%%%%--{{{1
--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%{{{2
--
--  Function: ADD
--
--
--  Description:
--     Adds two Big Numbers.
--
--  Parameters:
--     bnum1, bnum2 - Numbers to be added.
--     bnum3 - result
--
--  Returns:
--     0
--
--  Exit assertions:
--     bnum3 is the result of the sum.
--
--  %%%%%%%% --
--Funcao BigNum.add{{{2
function BigNumber.add( bnum1 , bnum2  )
	if bnum1 == nil or bnum2 == nil then
		error("Function BigNumber.add: parameter nil") ;
		return BigNumber.new()
	else
		local unit = math.max(bnum1.y,bnum2.y);
		bnum1.x = bnum1.x * math.pow(DEG,bnum1.y - unit);
		bnum1.y = unit;
		bnum2.x = bnum2.x * math.pow(DEG,bnum2.y - unit);
		bnum2.y = unit;
		bnum1.x = bnum1.x + bnum2.x;
		bnum1 = BigNumber.convert(bnum1);
		return bnum1;	
	end
end


--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%{{{2
--
--  Function: SUB
--
--
--  Description:
--     Subtracts two Big Numbers.
--
--  Parameters:
--     bnum1, bnum2 - Numbers to be subtracted.
--     bnum3 - result
--
--  Returns:
--     0
--
--  Exit assertions:
--     bnum3 is the result of the subtraction.
--
--  %%%%%%%% --
--Funcao BigNum.sub{{{2
function BigNumber.sub( bnum1 ,bnum2)
	if bnum1 == nil or bnum2 == nil then
		error("Function BigNumber.sub: parameter nil") ;
		return BigNumber.new();
	else
		local unit = math.max(bnum1.y,bnum2.y);
		bnum1.x = bnum1.x * math.pow(DEG,bnum1.y - unit);
		bnum1.y = unit;
		bnum2.x = bnum2.x * math.pow(DEG,bnum2.y - unit);
		bnum2.y = unit;
		bnum1.x = bnum1.x - bnum2.x;
		bnum1 = BigNumber.convert(bnum1);
		return bnum1;
	end
end


--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%{{{2
--
--  Function: MUL
--
--
--  Description:
--     Multiplies two Big Numbers.
--
--  Parameters:
--     bnum1, bnum2 - Numbers to be multiplied.
--     bnum3 - result
--
--  Returns:
--     0
--
--  Exit assertions:
--     bnum3 is the result of the multiplication.
--
--  %%%%%%%% --
--BigNum.mul{{{2
--can't be made in place
function BigNumber.mul( bnum1 , bnum2 )
	if bnum1 == nil or bnum2 == nil then
		error("Function BigNumber.mul: parameter nil") ;
		return BigNumber.new();
	else
		bnum1.x = bnum1.x * bnum2.x;
		if bnum1.x == 0 then
			return BigNumber.new();
		end
		bnum1.y = bnum1.y + bnum2.y;
		bnum1 = BigNumber.convert(bnum1);
		return bnum1; 
	end
end


--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%{{{2
--
--  Function: DIV
--
--
--  Description:
--     Divides bnum1 by bnum2.
--
--  Parameters:
--     bnum1, bnum2 - Numbers to be divided.
--     bnum3 - result
--     bnum4 - remainder
--
--  Returns:
--     0
--
--  Exit assertions:
--     bnum3 is the result of the division.
--     bnum4 is the remainder of the division.
--
--  %%%%%%%% --
--BigNum.div{{{2
function BigNumber.div( bnum1 , bnum2)
	if bnum1 == nil or bnum2 == nil  then
		error( "Function BigNumber.div: parameter nil" ) ;
		return BigNumber.new();
	else
		bnum1.x = bnum1.x / bnum2.x;
		if bnum1.x == 0 then
			return  BigNumber.new();
		end
		bnum1.y = bnum1.y - bnum2.y; 
		bnum1 = BigNumber.convert(bnum1);
		return  bnum1;
	end
end

--%%%%%%%%%%%%%%%%%%%% Compound Functions %%%%%%%%%%%%%%%%%%%%--{{{1





--%%%%%%%%%%%%%%%%%%%% Comparison Functions %%%%%%%%%%%%%%%%%%%%--{{{1

--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%{{{2
--
--  Function: EQ
--
--
--  Description:
--     Compares two Big Numbers.
--
--  Parameters:
--     bnum1, bnum2 - numbers
--
--  Returns:
--     Returns true if they are equal or false otherwise.
--
--  %%%%%%%% --
--BigNum.eq{{{2
function BigNumber.eq( bnum1 , bnum2 )
	if BigNumber.compare( bnum1 , bnum2 ) == 0 then
		return true ;
	else
		return false ;
	end
end

--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%{{{2
--
--  Function: LT
--
--
--  Description:
--     Verifies if bnum1 is lesser than bnum2.
--
--  Parameters:
--     bnum1, bnum2 - numbers
--
--  Returns:
--     Returns true if bnum1 is lesser than bnum2 or false otherwise.
--
--  %%%%%%%% --
--BigNum.lt{{{2
function BigNumber.lt( bnum1 , bnum2 )
	if BigNumber.compare( bnum1 , bnum2 ) == 2 then
		return true ;
	else
		return false ;
	end
end


--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%{{{2
--
--  Function: LE
--
--
--  Description:
--     Verifies if bnum1 is lesser or equal than bnum2.
--
--  Parameters:
--     bnum1, bnum2 - numbers
--
--  Returns:
--     Returns true if bnum1 is lesser or equal than bnum2 or false otherwise.
--
--  %%%%%%%% --
--BigNum.le{{{2
function BigNumber.le( bnum1 , bnum2 )
	local temp = -1 ;
	temp = BigNumber.compare( bnum1 , bnum2 )
	if temp == 0 or temp == 2 then
		return true ;
	else
		return false ;
	end
end


--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%{{{2
--
--  Function: LT
--
--
--  Description:
--     Verifies if bnum1 is greater than bnum2.
--
--  Parameters:
--     bnum1, bnum2 - numbers
--
--  Returns:
--     Returns true if bnum1 is lesser than bnum2 or false otherwise.
--
--  %%%%%%%% --
--BigNum.lt{{{2
function BigNumber.gt( bnum1 , bnum2 )
	if BigNumber.compare( bnum1 , bnum2 ) == 1 then
		return true ;
	else
		return false ;
	end
end


--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%{{{2
--
--  Function: LE
--
--
--  Description:
--     Verifies if bnum1 is greater or equal than bnum2.
--
--  Parameters:
--     bnum1, bnum2 - numbers
--
--  Returns:
--     Returns true if bnum1 is lesser or equal than bnum2 or false otherwise.
--
--  %%%%%%%% --
--BigNum.le{{{2
function BigNumber.ge( bnum1 , bnum2 )
	local temp = -1 ;
	temp = BigNumber.compare( bnum1 , bnum2 )
	if temp == 0 or temp == 1 then
		return true ;
	else
		return false ;
	end
end




--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%{{{2
--
--  Function: Compare
--
--
--  Description:
--     Compares values of bnum1 and bnum2.
--
--  Parameters:
--     bnum1, bnum2 - numbers
--
--  Returns:
--     1 - |bnum1| > |bnum2|
--     2 - |bnum1| < |bnum2|
--     0 - |bnum1| = |bnum2|
--
--  %%%%%%%% --
--BigNum.compare{{{2
function BigNumber.compare( bnum1 , bnum2 )
	--1是大于,2是小于,0是等于
	if bnum1 == nil or bnum2 == nil then
		error("Funtion BigNum.compare: parameter nil") ;
		return 3;
	else
		
		local l = bnum1-bnum2;
		if math.abs(l.x) <=0.01 then
			return 0;
	    elseif l.x >0.01 then
			return 1;	
		else 
			return 2;
		end		
	end
end





--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%{{{2


function BigNumber.convert( num)
	local bnum = {};
	setmetatable( bnum , BigNumber.mt ) ;
	local moreUnit = -LEN;
	if num.x ~= 0 or num.x ~= -0 then
		moreUnit = math.modf(math.log10(math.abs(num.x))) - LEN;
	end
	bnum.x = num.x / math.pow(DEG,moreUnit);
	bnum.y = num.y + moreUnit;
	if bnum.y <0 then
		bnum.x = bnum.x * math.pow(DEG,bnum.y);
		bnum.y = 0;
	end
	return bnum;
end



