import React from "react";
import { Button, Affix, Tooltip } from "antd";
import "./FloatingButtons.scss";

const FloatingButtons = ({ buttons }) => {
  return (
    <Affix offsetBottom={50} style={{ position: "fixed", bottom: 10, right: 30 }}>
      <div>
        {buttons.map((button) => {
          return (
            <Tooltip
              key={button.label}
              title={button.label}
              onClick={button.click}
            >
              <Button
                type="primary"
                shape="circle"
                icon={button.icon}
                size="large"
                onClick={button.clickAction}
                disabled={button.disabled}
              />
            </Tooltip>
          );
        })}
      </div>
    </Affix>
  );
};
export default FloatingButtons;
