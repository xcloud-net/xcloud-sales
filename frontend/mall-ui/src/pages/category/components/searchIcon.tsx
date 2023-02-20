import u from '@/utils';
import SearchIcon from '@mui/icons-material/Search';
import {
  Button,
  ClickAwayListener,
  IconButton,
  Input,
  InputAdornment,
  Paper,
  Slide,
  styled,
} from '@mui/material';
import { alpha } from '@mui/material/styles';
import { useState } from 'react';
import { history, useLocation } from 'umi';

const SearchbarStyle = styled(Paper)(({ theme }) => ({
  top: 0,
  left: 0,
  right: 0,
  zIndex: 99,
  display: 'flex',
  position: 'absolute',
  alignItems: 'center',
  height: u.config.app.layout.APPBAR_MOBILE,
  padding: theme.spacing(0, 3),
  boxShadow: `0 8px 16px 0 ${alpha(theme.palette.grey[500], 0.24)}`,
  backgroundColor: `${alpha(theme.palette.background.default, 1)}`,
  [theme.breakpoints.up('md')]: {
    height: u.config.app.layout.APPBAR_DESKTOP,
    padding: theme.spacing(0, 5),
  },
}));

export default function index() {
  const [isOpen, setOpen] = useState(false);
  const [query, setQuery] = useState('');

  const handleOpen = () => {
    setOpen((prev) => !prev);
  };

  const handleClose = () => {
    setOpen(false);
  };

  const location = useLocation();

  const triggerSearch = () => {
    handleClose();
    if (u.isEmpty(query)) {
      return;
    }
    setQuery('');
    if (u.pathEqual(location.pathname, '/goods')) {
      console.log('replace /goods');
      history.replace({
        pathname: '/goods',
        query: {
          kwd: query,
        },
      });
    } else {
      history.push({
        pathname: '/goods',
        query: {
          kwd: query,
        },
      });
    }
  };

  return (
    <ClickAwayListener onClickAway={handleClose}>
      <div>
        {!isOpen && (
          <IconButton size="large" color="inherit" onClick={handleOpen}>
            <SearchIcon width={20} height={20} />
          </IconButton>
        )}

        <Slide direction="down" in={isOpen} mountOnEnter unmountOnExit>
          <SearchbarStyle>
            <Input
              autoFocus
              fullWidth
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              disableUnderline
              placeholder="搜索产品..."
              onKeyUp={(e) => {
                if (u.lowerCase(e.key) == 'enter') {
                  triggerSearch();
                }
              }}
              startAdornment={
                <InputAdornment position="start">
                  <SearchIcon
                    sx={{ color: 'text.disabled', width: 20, height: 20 }}
                  />
                </InputAdornment>
              }
              sx={{ mr: 1, fontWeight: 'fontWeightBold' }}
            />
            <Button
              variant="contained"
              onClick={() => {
                triggerSearch();
              }}
            >
              搜索
            </Button>
          </SearchbarStyle>
        </Slide>
      </div>
    </ClickAwayListener>
  );
}
