import { useContext, useEffect, useCallback } from "react";
import { UNSAFE_NavigationContext as NavigationContext } from "react-router-dom";
import { FORM_TYPE } from "../../constants";
import store from "../../store";

export function useBlocker(blocker, when = true) {
  const { navigator } = useContext(NavigationContext);

  useEffect(() => {
    // console.log(`UsePrompt :${when ? "true" : "false"}`);
    if (!when) return;
    // const unblock = navigator.block((tx) => {
    //   const autoUnblockingTx = {
    //     ...tx,
    //     retry() {
    //       unblock();
    //       tx.retry();
    //     }
    //   };

    //   blocker(autoUnblockingTx);
    // });

    // return unblock;
    const push = navigator.push;

    navigator.push = (args) => {
      // const autoUnblockingTx = {
      //       ...tx,
      //       retry() {
      //         unblock();
      //         tx.retry();
      //       }
      //     };
      const result = blocker();
      if (result !== false) {
        push(args);
      }
    };

    return () => {
      navigator.push = push;
    };
  }, [navigator, blocker, when]);
}
export default function usePrompt(message, confirm, when = true) {
  // let isLoginExpired = store.getState().user.expiredDialogVisible;
  if (confirm) {
    when = true;
  }
  const blocker = useCallback(() => {
    let confirm = window.confirm(message);
    return confirm;
  }, [message]);
  useBlocker(blocker, when);
}
