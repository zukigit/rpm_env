import i18next from "i18next";

export const multipleStartUpLabel = (val) => {
    const t = i18next.t;
    val = parseInt(val);
    if(val === 0){
        return t("sel-yes");
    }else if(val === 1){
        return t("sel-skip");
    }else if(val === 2){
        return t("sel-waiting");
    }else{
        return '';
    }
}

export const jobnetTimeoutTypeLabel = (val) => {
    const t = i18next.t;
    val = parseInt(val);
    if(val === 0){
        return t("sel-warning");
    }else if(val === 1){
        return t("sel-jn-stop");
    }else{
        return '';
    }
}
