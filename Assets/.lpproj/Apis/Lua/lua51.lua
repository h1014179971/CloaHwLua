-- Lua 5.1 standard library

-- For more information, see <<Lua 5.1 Reference Manual>>.
-- https://www.lua.org/manual/5.1/manual.html

-- function assert(v, opt message)
function assert(v, message)
end

-- function collectgarbage(opt option, opt arg)
function collectgarbage(option, arg)
end

-- function dofile(opt filename)
function dofile(filename)
end

-- function error(message, opt level)
function error(message, level)
end

-- deprecated
function gcinfo()
end

-- function getfenv(opt f)
function getfenv(f)
end

function getmetatable(object)
end

function ipairs(t)
end

-- function load(func, opt chunkame)
function load(func, chunkame)
end

-- function loadfile(opt filename)
function loadfile(filename)
end

-- function loadstring(string, opt chunkname)
function loadstring(string, chunkname)
end

function module(name, ...)
end

-- deprecated
function newproxy()
end

-- function next(table, opt index)
function next(table, index)
end

function pairs(t)
end

function pcall(f, ...)
end

function print(...)
end

function rawequal(v1, v2)
end

function rawget(table, index)
end

function rawset(table, index, value)
end

function require(modname)
end

function select(index, ...)
end

function setfenv(f, table)
end

function setmetatable(table, metatable)
end

-- function tonumber(e, opt base)
function tonumber(e, base)
end

function tostring(e)
end

function type(v)
end

-- function unpack(list, opt i, opt j)
function unpack(list, i, j)
end

function xpcall(f, err, ...)
end

coroutine = {}

function coroutine.create(f)
end

function coroutine.resume(co, ...)
end

function coroutine.running()
end

function coroutine.status(co)
end

function coroutine.wrap(f)
end

function coroutine.yield(...)
end

package = {}

function package.loadlib(libname, funcname)
end

function package.seeall(module)
end

string = {}

-- function string.byte(s, opt i, opt j)
function string.byte(s, i, j)
end

function string.char(...)
end

-- function string.dump(func)
function string.dump(func)
end

-- function string.find(s, pattern, opt init, opt plain)
function string.find(s, pattern, init, plain)
end

function string.format(formatstring, ...)
end

-- deprecated, using string.gmatch() instead
function string.gfind()
end

function string.gmatch(s, pattern)
end

-- function string.gsub(s, pattern, repl, opt n)
function string.gsub(s, pattern, repl, n)
end

function string.len(s)
end

function string.lower(s)
end

-- function string.match(s, pattern, opt init)
function string.match(s, pattern, init)
end

-- function string.rep(s, n)
function string.rep(s, n)
end

function string.reverse(s)
end

-- function string.sub(s, i, opt j)
function string.sub(s, i, j)
end

function string.upper(s)
end

table = {}

-- function table.concat(table, opt sep, opt i, opt j)
function table.concat(table, sep, i, j)
end

-- deprecated
function table.foreach()
end

-- deprecated
function table.foreachi()
end

-- deprecated, using length operator (#) instead
function table.getn()
end

-- function table.insert(table, opt pos, value)
function table.insert(table, pos, value)
end

function table.maxn(table)
end

-- function table.remove(table, opt pos)
function table.remove(table, pos)
end

-- deprecated
function table.setn()
end

-- function table.sort(list, opt comp)
function table.sort(list, comp)
end

math = {}

function math.abs(x)
end

function math.acos(x)
end

function math.asin(x)
end

function math.atan(x)
end

function math.atan2(y, x)
end

function math.ceil(x)
end

function math.cos(x)
end

function math.cosh(x)
end

function math.deg(x)
end

function math.exp(x)
end

function math.floor(x)
end

function math.fmod(x, y)
end

function math.frexp(x)
end

function math.ldexp(m, e)
end

function math.log(x)
end

function math.log10(x)
end

function math.max(x, ...)
end

function math.min(x, ...)
end

-- deprecated, using math.fmod() instead
function math.mod()
end

function math.modf(x)
end

function math.pow(x, y)
end

function math.rad(x)
end

-- function math.random(opt m, opt n)
function math.random(m, n)
end

function math.randomseed(x)
end

function math.sin(x)
end

function math.sinh(x)
end

function math.sqrt(x)
end

function math.tan(x)
end

function math.tanh(x)
end

math.huge = 0

math.pi = 0

io = {}

-- function io.close(opt file)
function io.close(file)
end

function io.flush()
end

-- function io.input(opt file)
function io.input(file)
end

-- function io.lines(opt filename)
function io.lines(filename)
end

-- function io.open(filename, opt mode) : File
function io.open(filename, mode)
end

-- function io.output(opt file)
function io.output(file)
end

-- function io.popen(prog, opt mode)
function io.popen(prog, mode)
end

function io.read(...)
end

function io.tmpfile()
end

function io.type(obj)
end

function io.write(...)
end

local File = {}

function File:close()
end

function File:flush()
end

function File:lines()
end

function File:read(...)
end

-- function File:seek(opt whence, opt offset)
function File:seek(whence, offset)
end

-- function File:setvbuf(mode, opt size)
function File:setvbuf(mode, size)
end

function File:write(...)
end

os = {}

function os.clock()
end

-- function os.date(opt format, opt time)
function os.date(format, time)
end

function os.difftime(t2, t1)
end

-- function os.execute(opt command)
function os.execute(command)
end

-- function os.exit(opt code)
function os.exit(code)
end

function os.getenv(varname)
end

function os.remove(filename)
end

function os.rename(oldname, newname)
end

-- function os.setlocale(locale, opt category)
function os.setlocale(locale, category)
end

-- function os.time(opt table)
function os.time(table)
end

function os.tmpname()
end

debug = {}

function debug.debug()
end

function debug.getfenv(o)
end

-- function debug.gethook(opt thread)
function debug.gethook(thread)
end

-- function debug.getinfo(opt thread, func, opt what)
function debug.getinfo(thread, func, what)
end

-- function debug.getlocal(opt thread, level, local_name)
function debug.getlocal(thread, level, local_name)
end

function debug.getmetatable(object)
end

function debug.getregistry()
end

function debug.getupvalue(func, up)
end

function debug.setfenv(object, table)
end

-- function debug.sethook(opt thread, hook, mask, opt count)
function debug.sethook(thread, hook, mask, count)
end

-- function debug.setlocal(opt thread, level, local_name, value)
function debug.setlocal(thread, level, local_name, value)
end

function debug.setmetatable(object, table)
end

function debug.setupvalue(func, up, value)
end

-- function debug.traceback(opt thead, opt message, opt level)
function debug.traceback(thead, message, level)
end