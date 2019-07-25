
export function getServerAddress() {
    if (typeof webpackHotUpdate !== 'undefined') {
         console.log('In Dev Mode');
        return "http://localhost:5012";
    } else {
        console.log('In Release Mode');
        return "";
    }
}