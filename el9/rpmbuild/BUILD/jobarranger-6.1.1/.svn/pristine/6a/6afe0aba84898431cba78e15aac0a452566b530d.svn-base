import React from "react";
import { Layout } from "antd";
import SideBar from "../sidebar/Sidebar";
import BreadcrumbsNav from "../breadcrumbsNav/BreadcrumbsNav";
import Foot from "../footer/Footer";
import { Outlet } from "react-router-dom";

const { Header, Footer, Sider, Content } = Layout;
function MainLayout() {

  return (
    <Layout style={{ minHeight: "100vh" }}>
      <SideBar />
      <Layout>
        <Header
          style={{
            backgroundColor: "#F4F5F7",
            height: "50px",
            padding: "0 32px",
          }}
        >
          <BreadcrumbsNav />
        </Header>
        <Content style={{ padding: "0rem 1rem 1.5rem 1rem" }}>
          <Outlet />
        </Content>
        <Footer style={{ textAlign: "center" }}>
          <Foot />
        </Footer>
      </Layout>
    </Layout>
  );
}

export default MainLayout;
