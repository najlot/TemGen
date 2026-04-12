import { dotnet } from './_framework/dotnet.js'

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

globalThis.temgenBrowserGetLocalStorage = (key) => globalThis.localStorage.getItem(key);
globalThis.temgenBrowserSetLocalStorage = (key, value) => globalThis.localStorage.setItem(key, value);
globalThis.temgenBrowserRemoveLocalStorage = (key) => globalThis.localStorage.removeItem(key);
globalThis.temgenBrowserGetProtocol = () => globalThis.location.protocol;
globalThis.temgenBrowserGetHostName = () => globalThis.location.hostname;

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const config = dotnetRuntime.getConfig();

await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
