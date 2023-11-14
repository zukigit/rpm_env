export const getShiftJISByteLength = (str) => {
    if(str){
        if(str.length > 0){
            return str.replace(
                /[^\x00-\x80｡｢｣､･ｦｧｨｩｪｫｬｭｮｯｰｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝ ﾞ ﾟ]/g,
                "xx"
            ).length;
        }
    }
    return 0;
}