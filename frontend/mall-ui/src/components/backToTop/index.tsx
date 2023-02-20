import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import { Tooltip } from '@mui/material';
import Box from '@mui/material/Box';
import Fab from '@mui/material/Fab';
import Zoom from '@mui/material/Zoom';
import useScrollTrigger from '@mui/material/useScrollTrigger';
import * as React from 'react';

interface Props {
  children: React.ReactElement;
  hide?: boolean;
}

export default function ScrollTop(props: Props) {
  const { hide } = props;
  // Note that you normally won't need to set the window ref as useScrollTrigger
  // will default to window.
  // This is only being set here because the demo is in an iframe.
  const trigger = useScrollTrigger({
    disableHysteresis: true,
    threshold: 100,
  });

  return (
    <Zoom in={trigger && !hide}>
      <Box
        onClick={() => {
          window &&
            window.scrollTo({
              left: 0,
              top: 0,
            });
        }}
        role="presentation"
        sx={{
          position: 'fixed',
          bottom: 70,
          right: 16,
          zIndex: 9999,
          display: 'inline-block',
        }}
      >
        <Tooltip title="返回顶部">
          <Fab sx={{}} color="primary" size="small">
            <KeyboardArrowUpIcon />
          </Fab>
        </Tooltip>
      </Box>
    </Zoom>
  );
}
