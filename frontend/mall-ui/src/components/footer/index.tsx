import FavoriteIcon from '@mui/icons-material/Favorite';
import { Typography } from '@mui/material';

export default function index() {
  return (
    <>
      <Typography
        variant="body2"
        color="text.secondary"
        align="center"
        sx={{ mt: 2, mb: 1, fontSize: 10 }}
      >
        {'Provided by'} <strong>Douya</strong> {'with love.'}
        {'{'}
        <FavoriteIcon sx={{ color: 'red' }} fontSize="small" />
        {'}'}
      </Typography>
    </>
  );
}
