import React from "react";
import { Breadcrumb } from "antd";
import { Link, useLocation, matchPath } from "react-router-dom";
import "./BreadcrumbsNav.scss";
import { useTranslation } from "react-i18next";
const BreadcrumbsNav = () => {
  const { t } = useTranslation();
  const url_path = "/jobarranger";
  var pathArray = window.location.pathname.split("/");
  var ObjectId;
  var ObjectDate;

  if (pathArray[2] === "object-version") {
    ObjectId = pathArray[pathArray.length - 1];
  } else {
    ObjectDate = pathArray[pathArray.length - 1];
    ObjectId = pathArray[pathArray.length - 2];
  }
  const routes = [
    {
      path: `/`,
      breadcrumbName: t("nav-home"),
    },
    {
      path: `/object-list/calendar/public`,
      breadcrumbName: t("nav-public-calendar"),
      child: [
        {
          path: `/calendar/create/public`,
          breadcrumbName: t("nav-calendar-create"),
        },

        {
          path: `/object-version/calendar/public/` + ObjectId,
          breadcrumbName: t("nav-calendar-version"),
          child: [
            {
              path: "/calendar/edit/public/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-calendar-edit"),
            },
            {
              path:
                "/calendar/new-object/public/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-calendar-new-obj"),
            },
            {
              path:
                "/calendar/new-version/public/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-calendar-new-ver"),
            },
          ],
        },
      ],
    },
    {
      path: `/object-list/calendar/private`,
      breadcrumbName: t("nav-private-calendar"),
      child: [
        {
          path: `/calendar/create/private`,
          breadcrumbName: t("nav-calendar-create"),
        },
        {
          path: `/object-version/calendar/private/` + ObjectId,
          breadcrumbName: t("nav-calendar-version"),
          child: [
            {
              path: "/calendar/edit/private/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-calendar-edit"),
            },
            {
              path:
                "/calendar/new-object/private/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-calendar-new-obj"),
            },
            {
              path:
                "/calendar/new-version/private/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-calendar-new-ver"),
            },
          ],
        },
      ],
    },
    {
      path: `/object-list/filter/public`,
      breadcrumbName: t("nav-public-filter"),
      child: [
        {
          path: `/filter/create/public`,
          breadcrumbName: t("nav-filter-create"),
        },
        {
          path: `/object-version/filter/public/` + ObjectId,
          breadcrumbName: t("nav-filter-version"),
          child: [
            {
              path: "/filter/edit/public/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-filter-edit"),
            },
            {
              path: "/filter/new-object/public/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-filter-new-obj"),
            },
            {
              path: "/filter/new-version/public/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-filter-new-ver"),
            },
          ],
        },
      ],
    },
    {
      path: `/object-list/filter/private`,
      breadcrumbName: t("nav-private-filter"),
      child: [
        {
          path: `/filter/create/private`,
          breadcrumbName: t("nav-filter-create"),
        },
        {
          path: `/object-version/filter/private/` + ObjectId,
          breadcrumbName: t("nav-filter-version"),
          child: [
            {
              path: "/filter/edit/private/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-filter-edit"),
            },
            {
              path: "/filter/new-object/private/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-filter-new-obj"),
            },
            {
              path:
                "/filter/new-version/private/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-filter-new-ver"),
            },
          ],
        },
      ],
    },
    {
      path: `/object-list/schedule/public`,
      breadcrumbName: t("nav-public-schedule"),
      child: [
        {
          path: `/schedule/create/public`,
          breadcrumbName: t("nav-schedule-create"),
        },
        {
          path: `/object-version/schedule/public/` + ObjectId,
          breadcrumbName: t("nav-schedule-version"),
          child: [
            {
              path: "/schedule/edit/public/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-schedule-edit"),
            },
            {
              path:
                "/schedule/new-object/public/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-schedule-new-obj"),
            },
            {
              path:
                "/schedule/new-version/public/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-schedule-new-ver"),
            },
          ],
        },
      ],
    },
    {
      path: `/object-list/schedule/private`,
      breadcrumbName: t("nav-private-schedule"),
      child: [
        {
          path: `/schedule/create/private`,
          breadcrumbName: t("nav-schedule-create"),
        },
        {
          path: `/object-version/schedule/private/` + ObjectId,
          breadcrumbName: t("nav-schedule-version"),
          child: [
            {
              path: "/schedule/edit/private/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-schedule-edit"),
            },
            {
              path:
                "/schedule/new-object/private/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-schedule-new-obj"),
            },
            {
              path:
                "/schedule/new-version/private/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-schedule-new-ver"),
            },
          ],
        },
      ],
    },
    {
      path: `/object-list/jobnet/public`,
      breadcrumbName: t("nav-public-jobnet"),
      child: [
        {
          path: `/jobnet/create/public`,
          breadcrumbName: t("nav-jobnet-create"),
        },
        {
          path: `/object-version/jobnet/public/` + ObjectId,
          breadcrumbName: t("nav-jobnet-version"),
          child: [
            {
              path: "/jobnet/edit/public/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-jobnet-edit"),
            },
            {
              path: "/jobnet/new-object/public/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-jobnet-new-obj"),
            },
            {
              path: "/jobnet/new-version/public/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-jobnet-new-ver"),
            },
          ],
        },
      ],
    },
    {
      path: `/object-list/jobnet/private`,
      breadcrumbName: t("nav-private-jobnet"),
      child: [
        {
          path: `/jobnet/create/private`,
          breadcrumbName: t("nav-jobnet-create"),
        },
        {
          path: `/object-version/jobnet/private/` + ObjectId,
          breadcrumbName: t("nav-jobnet-version"),
          child: [
            {
              path: "/jobnet/edit/private/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-jobnet-edit"),
            },
            {
              path: "/jobnet/new-object/private/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-jobnet-new-obj"),
            },
            {
              path:
                "/jobnet/new-version/private/" + ObjectId + "/" + ObjectDate,
              breadcrumbName: t("nav-jobnet-new-ver"),
            },
          ],
        },
      ],
    },
    {
      path: `/job-execution-management`,
      breadcrumbName: t("job-exe-mng"),
    },
    {
      path: `/job-execution-result`,
      breadcrumbName: t("job-exe-res"),
    },
    {
      path: `/general-setting`,
      breadcrumbName: t("nav-general-setting"),
    },
    {
      path: `/lock-management`,
      breadcrumbName: t("nav-lock-management"),
    },
  ];

  let location = useLocation();
  let url_routes = [];
  // all routes to reder.

  const matchChildParent = (route) => {
    let paths = [];
    //check if route matches the default.
    if (Array.isArray(route)) {
      route.forEach((e) => {
        let tmp_child_path = matchChildParent(e);
        if (tmp_child_path.length) {
          paths = [...paths, ...tmp_child_path];
        }
      });
    } else {
      let match = matchPath(
        { path: route.path, end: "false" },
        location.pathname
      );
      if (match) {
        //push route to the match array list.
        paths.push(route);
      } else if (route.child) {
        //parnt does not match, then check for the child(if child exist)
        let tmp_path = matchChildParent(route.child);
        if (tmp_path.length) {
          paths = [...paths, ...tmp_path];
          paths.push(route);
        }
      }
    }
    return paths;
  };
  const itemRender = (paths) => {
    //if path matches home, only home is rendered.
    let home_match = matchPath(
      { path: "/", end: "false", strict: "true" },
      location.pathname
    );
    if (home_match) {
      return (
        <Breadcrumb.Item key="home">
          <span>{t("nav-home")}</span>
        </Breadcrumb.Item>
      );
    } else {
      //return (<Link to={url_path}>Home</Link>);
      //if not home route, render home as link, and it's child as link.
      let child_str = [];
      child_str.push(
        <Breadcrumb.Item key="Home">
          <Link to={"/"}>{t("nav-home")}</Link>
        </Breadcrumb.Item>
      );
      let path_pop = paths.pop();
      while (path_pop) {
        if (paths.length) {
          child_str.push(
            <Breadcrumb.Item key={path_pop.breadcrumbName}>
              <Link to={path_pop.path}>{path_pop.breadcrumbName}</Link>
            </Breadcrumb.Item>
          );
        } else {
          child_str.push(
            <Breadcrumb.Item key={path_pop.breadcrumbName}>
              <span>{path_pop.breadcrumbName}</span>
            </Breadcrumb.Item>
          );
        }
        path_pop = paths.pop();
      }
      return child_str;
    }
  };
  routes.forEach((route) => {
    let tmp_url_routes = matchChildParent(route);
    if (tmp_url_routes.length) {
      url_routes = [...url_routes, ...tmp_url_routes];
      return;
    }
  });
  return (
    // <div>{parameters.map((reptile) => <li>{reptile}</li>)}</div>
    <Breadcrumb separator="➤">{itemRender(url_routes)}</Breadcrumb>
  );
};

export default BreadcrumbsNav;
