import SearchIcon from '@mui/icons-material/Search';
import { Button, IconButton, InputBase, Paper } from '@mui/material';

export default function CustomizedInputBase(props: {
  keywords?: string;
  onSearch?: any;
  onChange?: any;
}) {
  const { keywords, onSearch, onChange } = props;

  const triggerSearch = () => {
    onSearch && onSearch();
  };

  return (
    <>
      <Paper
        sx={{
          display: 'flex',
          alignItems: 'center',
          border: (theme) => `2px solid ${theme.palette.error.main}`,
        }}
        component="form"
        onSubmit={(e) => {
          e.preventDefault();
          triggerSearch();
        }}
      >
        <IconButton>
          <SearchIcon />
        </IconButton>
        <InputBase
          sx={{ ml: 1, flex: 1 }}
          value={keywords}
          onChange={(e) => {
            onChange && onChange(e.target.value);
          }}
          placeholder="搜索商品..."
        />
        <Button
          color="error"
          onClick={() => {
            triggerSearch();
          }}
        >
          搜 索
        </Button>
      </Paper>
    </>
  );
}
