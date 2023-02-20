import { Close } from '@mui/icons-material';
import { Box, Dialog, Fab, Slide } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { TransitionProps } from '@mui/material/transitions';
import useMediaQuery from '@mui/material/useMediaQuery';
import Zoom from '@mui/material/Zoom';
import * as React from 'react';

const Transition = React.forwardRef(function Transition(
  props: TransitionProps & {
    children: React.ReactElement<any, any>;
  },
  ref: React.Ref<unknown>,
) {
  return <Slide direction="up" ref={ref} {...props} />;
});

const index = function (props: any) {
  const { open, onClose, children } = props;

  var dialogProps = {};
  const theme = useTheme();
  const largeScreen = useMediaQuery(theme.breakpoints.up('sm'));
  if (largeScreen) {
    dialogProps = {
      ...dialogProps,
      maxWidth: 'sm',
      fullWidth: true,
    };
  } else {
    dialogProps = {
      ...dialogProps,
      fullScreen: true,
    };
  }

  return (
    <>
      <Dialog
        TransitionComponent={Transition}
        //disableEscapeKeyDown
        keepMounted
        sx={{
          m: 0,
          p: 0,
          //zIndex: 10,
        }}
        scroll="body"
        open={open}
        onClose={() => {
          onClose && onClose();
        }}
        {...dialogProps}
      >
        <Zoom in={open} style={{ transitionDelay: open ? '300ms' : '0ms' }}>
          <Box
            sx={{
              position: 'fixed',
              display: 'inline',
              right: '30px',
              top: '30px',
              zIndex: 1,
            }}
          >
            <Fab
              sx={{}}
              size="small"
              color="error"
              onClick={() => {
                onClose && onClose();
              }}
            >
              <Close />
            </Fab>
          </Box>
        </Zoom>
        <Box
          sx={{
            m: 0,
            p: 0,
          }}
        >
          {children}
        </Box>
      </Dialog>
    </>
  );
};

export default index;
