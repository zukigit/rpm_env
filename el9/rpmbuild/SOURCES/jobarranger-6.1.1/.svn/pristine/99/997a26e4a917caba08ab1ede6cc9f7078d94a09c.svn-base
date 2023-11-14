import { Button, Input, Checkbox, Spin } from "antd";
import { useState } from "react";
import { Modal } from "antd";
import ImportService from "../../services/ImportService";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import {
  alertError,
  alertSuccess,
} from "../../components/dialogs/CommonDialog";
import "./ImportDialog.scss";
import {
  getAllObjectList,
  getAllObjectVersion,
} from "../../store/ObjectListSlice";
export default function ImportDialog({ isOpen, onHandle, upload }) {
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const [selectedFile, setSelectedFile] = useState();
  const [isSelected, setIsSelected] = useState(false);
  const [fileName, setFileName] = useState();
  const [checked, setChecked] = useState(false);
  const [loading, setLoading] = useState(false);

  const clickReference = () => {
    if (upload === "HomeFileUpload") {
      document.getElementById("HomeFileUpload").click();
    } else {
      document.getElementById("fileUpload").click();
    }
  };

  const reloadList = () => {
    var pathArray = window.location.pathname.split("/");
    var category;
    var publicType;
    var id;

    if (pathArray[2] === "object-version") {
      category = pathArray[3];
      publicType = pathArray[4];
      id = pathArray[5];
      dispatch(
        getAllObjectVersion({
          category: category,
          publicType: publicType,
          objectId: id,
        })
      );
    } else if (pathArray[2] === "object-list") {
      category = pathArray[3];
      publicType = pathArray[4];
      dispatch(
        getAllObjectList({ category: category, publicType: publicType })
      );
    }
  };

  const onChange = (e) => {
    setChecked(e.target.checked);
  };
  function setSelectedData(event) {
    setFileName(event.target.files[0].name);
    setSelectedFile(event.target.files[0]);
    setIsSelected(true);
    event.target.value = null;
  }
  const changeHandler = (event) => {
    setSelectedData(event);
  };

  const handleSubmission = (e) => {
    if (!isSelected) {
      setLoading(false);
      alertError(t("title-error"), t("err-msg-input-file"));
      return;
    }
    const fileReader = new FileReader();
    fileReader.readAsText(selectedFile);

    fileReader.onload = function (e) {
      var data = {
        fileContent: JSON.stringify(e.target.result),
        chkOverwrite: checked,
      };
      ImportService.importXML(data)
        .then((response) => {
          setLoading(false);
          switch (response.detail.message) {
            case "SUCCESS_MSG":
              setSelectedFile(null);
              setIsSelected(false);
              setChecked(false);
              setFileName();
              onHandle();
              reloadList();
              alertSuccess(t("title-success"), t("info-msg-import-suc"));
              break;
            case "ERR_XML_FORMAT":
              alertError(t("title-error"), t("err-msg-err-load"));
              break;
            case "ERR_USER_INFO":
              alertError(t("title-error"), t("err-msg-user-info"));
              break;
            case "ERR_PERMISSION":
              alertError(t("title-error"), t("err-msg-no-permission"));
              break;
            case "ERR_DB":
              alertError(t("title-error"), t("err-msg-db-exec-err"));
              break;
            case "ERR_IMPORT":
              alertError(t("title-error"), t("err-msg-cannot-import"));
              break;
            case "ERR_REGISTERED":
              alertError(t("title-error"), t("err-msg-alr-reg"));
              break;
          }
        })
        .catch((error) => {
          setLoading(false);
          alertError(t("title-error"), t("lab-server-error"));
        });
    };

    fileReader.onerror = function (e) {
      setLoading(false);
      if (fileReader.error.name === "NotFoundError") {
        alertError(t("title-error"), t("err-msg-file-not-exist"));
      } else {
        alertError(t("title-error"), t("err-msg-err-load"));
      }
    };
  };

  return (
    <Modal
      width="500px"
      className="import-model"
      bodyStyle={{ height: "170px" }}
      title={t("title-import")}
      visible={isOpen}
      confirmLoading={loading}
      centered={true}
      okText={t("btn-ok")}
      cancelText={t("btn-cancel")}
      maskClosable={false}
      onOk={() => {
        setLoading(true);
        handleSubmission();
      }}
      onCancel={() => {
        onHandle();
        setSelectedFile();
        setIsSelected(false);
        setChecked(false);
        setFileName();
      }}
    >
      <div>
        <input
          type="file"
          id={upload}
          name="fileupload"
          accept=".xml"
          style={{ display: "none" }}
          onChange={changeHandler}
        />
        <div style={{ marginBottom: "30px" }}>{t("lab-import-info")}</div>
        <div style={{ display: "flex" }}>
          <Input disabled id="fileName" value={fileName} />

          <Button type="button" id="reference" onClick={clickReference}>
            {t("btn-ref")}
          </Button>
        </div>

        <div style={{ marginTop: "5px" }}>
          <Checkbox checked={checked} onChange={onChange} id="overwriteFlag">
            {t("chk-overwrite")}
          </Checkbox>
        </div>
      </div>
    </Modal>
  );
}
