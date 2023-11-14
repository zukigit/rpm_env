import React, { useEffect, useRef } from "react";
import { Link } from "react-router-dom";
import {
  UserOutlined,
  MenuOutlined,
  TableOutlined,
  BarsOutlined,
  CalendarOutlined,
  ClockCircleOutlined,
  DeploymentUnitOutlined,
  ProjectFilled,
  SettingFilled,
  UnlockFilled,
  FileSyncOutlined,
  ImportOutlined,
  ExportOutlined,
  LogoutOutlined,
  CalendarFilled,
  ClockCircleFilled,
  ApartmentOutlined,
  ScheduleOutlined,
  ScheduleFilled,
} from "@ant-design/icons";
import { Layout, Menu } from "antd";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import "./Sidebar.scss";
import ImportDialog from "../../../views/import/ImportDialog";
import Logout from "../../../views/logout/Logout";
import Export from "../../../views/export/Export";
import { useSelector } from "react-redux";
import { SESSION_STORAGE } from "../../../constants";

const { Sider } = Layout;

const Sidebar = () => {
  const tmp = useRef(0);
  tmp.current++;
  const { t } = useTranslation();
  const [sidebarCollapsed, setSidebarCollapsed] = useState(
    sessionStorage.getItem(SESSION_STORAGE.IS_SIDEBAR_COLLAPSE)
      ? sessionStorage.getItem(SESSION_STORAGE.IS_SIDEBAR_COLLAPSE) === "true"
      : true
  );
  const [status, setStatus] = useState(false);
  const [show, setShow] = useState(false);
  const [exportOpen, setExportOpen] = useState(false);
  const [openedMenu, setOpenedMenu] = useState([]);
  const currentOpenedMenu = useRef([]);
  const collapseChanged = useRef(false);
  const userInfo = useSelector((state) => state["user"].userInfo);
  const [fileupload, setFileUpload] = useState("");
  function getItem(label, key, icon, children) {
    return {
      key,
      icon,
      children,
      label,
    };
  }
  function handleExport() {
    setExportOpen(!exportOpen);
  }
  function onLogoutCancel() {
    setShow(!show);
  }

  function onHandle() {
    setStatus(!status);
  }
  const toggle = () => {
    sessionStorage.setItem(
      SESSION_STORAGE.IS_SIDEBAR_COLLAPSE,
      !sidebarCollapsed
    );
    setSidebarCollapsed(!sidebarCollapsed);
  };

  const openChangeHandler = (v) => {
    if (collapseChanged.current && sidebarCollapsed) {
      collapseChanged.current = false;
    } else {
      if (sidebarCollapsed && v.length > 2) {
        v.splice(1, 1);
        currentOpenedMenu.current = v;
      }
      currentOpenedMenu.current = v;
      setOpenedMenu(v);
    }
  };
  useEffect(() => {
    if (sidebarCollapsed) {
      setOpenedMenu([]);
      collapseChanged.current = true;
    } else {
      setOpenedMenu(currentOpenedMenu.current);
      collapseChanged.current = true;
    }
  }, [sidebarCollapsed]);
  const items = [
    getItem(
      <div style={{ width: "100%", height: "100%" }} onClick={toggle}>
        {t("sidebar-title")}
      </div>,
      "manager",
      <MenuOutlined
        style={{ fontSize: "20px", color: "white" }}
        onClick={toggle}
      />
    ),
    getItem(
      `${userInfo.userName}`,
      "name",
      <UserOutlined style={{ fontSize: "20px", color: "white" }} />
    ),
    getItem(
      t("menu-object-management"),
      "object-management",
      <BarsOutlined />,
      [
        getItem(t("menu-cal"), "calendar", <CalendarOutlined />, [
          getItem(
            <Link to={"/object-list/calendar/public"}>
              {t("menu-pub-cal")}
            </Link>,
            "public-calendar",
            <CalendarFilled style={{ fontSize: "13px" }} />
          ),
          getItem(
            <Link to={"/object-list/calendar/private"}>
              {t("menu-prv-cal")}
            </Link>,
            "private-calendar",
            <CalendarOutlined style={{ fontSize: "13px" }} />
          ),
          getItem(
            <Link to={"/object-list/filter/public"}>{t("menu-pub-flt")}</Link>,
            "public-filter",
            <ClockCircleFilled style={{ fontSize: "13px" }} />
          ),
          getItem(
            <Link to={"/object-list/filter/private"}>{t("menu-prv-flt")}</Link>,
            "private-filter",
            <ClockCircleOutlined style={{ fontSize: "13px" }} />
          ),
        ]),

        getItem(t("menu-schd"), "schedule", <ScheduleOutlined />, [
          getItem(
            <Link to={"/object-list/schedule/public"}>
              {t("menu-pub-schd")}
            </Link>,
            "public-schedule",
            <ScheduleFilled style={{ fontSize: "13px" }} />
          ),
          getItem(
            <Link to={"/object-list/schedule/private"}>
              {t("menu-prv-schd")}
            </Link>,
            "private-schedule",
            <ScheduleOutlined style={{ fontSize: "13px" }} />
          ),
        ]),

        getItem(t("menu-jobnet"), "jobnet", <DeploymentUnitOutlined />, [
          getItem(
            <Link to={"/object-list/jobnet/public"}>
              {t("menu-pub-jobnet")}
            </Link>,
            "public-jobnet",
            <ApartmentOutlined style={{ fontSize: "13px" }} />
          ),
          getItem(
            <Link to={"/object-list/jobnet/private"}>
              {t("menu-prv-jobnet")}
            </Link>,
            "private-jobnet",
            <DeploymentUnitOutlined style={{ fontSize: "13px" }} />
          ),
        ]),
      ]
    ),
    getItem(
      <Link to={"/job-execution-management"} style={{ color: "white" }}>
        {t("menu-job-exe-mgt")}
      </Link>,
      "exec-management",
      <TableOutlined />
    ),

    getItem(
      <Link to={"/job-execution-result"} style={{ color: "white" }}>
        {t("menu-job-exe-result")}
      </Link>,
      "exec-result",
      <ProjectFilled />
    ),

    getItem(
      <Link to={"/general-setting"} style={{ color: "white" }}>
        {t("menu-gen-setting")}
      </Link>,
      "general-setting",
      <SettingFilled />
    ),

    getItem(
      <Link to={"/lock-management"} style={{ color: "white" }}>
        {t("menu-lock-management")}
      </Link>,
      "lock-management",
      <UnlockFilled />
    ),

    getItem(t("menu-export-import"), "import-export", <FileSyncOutlined />, [
      getItem(
        <span onClick={handleExport}>{t("txt-exp-all")}</span>,
        "export",
        <ExportOutlined onClick={handleExport} />
      ),
      getItem(
        <span
          onClick={() => {
            setStatus(true);
            setFileUpload("fileUpload");
          }}
        >
          {t("btn-import")}
        </span>,

        "import",
        <ImportOutlined
          onClick={() => {
            setStatus(true);
            setFileUpload("fileUpload");
          }}
        />
      ),
    ]),

    getItem(
      <div
        style={{ width: "100%", height: "100%" }}
        onClick={() => {
          setShow(true);
        }}
      >
        {t("btn-logout")}
      </div>,

      "logout",
      <LogoutOutlined
        onClick={() => {
          setShow(true);
        }}
      />
    ),
  ];
  return (
    <Sider
      trigger={null}
      collapsible
      breakpoint="lg"
      collapsedWidth="50"
      width="245"
      collapsed={sidebarCollapsed}
      // onCollapse={(value) => collapseAction(value)}
    >
      <Menu
        theme="dark"
        mode="inline"
        //inlineCollapsed={sidebarCollapsed}
        //defaultSelectedKeys={1}
        //defaultOpenKeys={openedMenu}
        onOpenChange={openChangeHandler}
        openKeys={openedMenu}
        style={{
          height: "100vh",
          width: sidebarCollapsed ? "50px" : "245px",
          position: "fixed",
          overflow: "auto",
        }}
        items={items}
      />
      <Export exportOpen={exportOpen} handleExport={handleExport} />
      <ImportDialog isOpen={status} onHandle={onHandle} upload={fileupload} />
      <Logout isOpen={show} onCancel={onLogoutCancel} />
    </Sider>
  );
};

export default Sidebar;
