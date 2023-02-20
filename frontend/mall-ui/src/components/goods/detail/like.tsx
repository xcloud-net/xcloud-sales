import XLoginCheck from '@/components/login/check';
import API from '@/services/storeApp';
import FavoriteIcon from '@mui/icons-material/Favorite';
import { Box, Fab } from '@mui/material';
import * as React from 'react';
import { useModel } from 'umi';
import u from '@/utils';
import { GoodsDto } from '@/utils/models';

const index = function FloatingActionButtons(props: {
  model: GoodsDto;
  ok: any;
}) {
  const { model, ok } = props;
  const [loading, _loading] = React.useState(false);
  const storeAppAccountModel = useModel('storeAppAccount');

  const triggerRefresh = (liked: boolean) => ok && ok(liked);

  const addCollectOrNot = () => {
    if (!model.Id || !storeAppAccountModel.isUserLogin()) {
      return;
    }

    if (model.IsFavorite) {
      _loading(true);
      API.removeFavorite(model.Id)
        .then((res) => {
          u.handleResponse(res, () => {
            u.error('取消收藏');
            triggerRefresh(false);
          });
        })
        .finally(() => {
          _loading(false);
        });
    } else {
      _loading(true);
      API.addFavorite(model.Id)
        .then((res) => {
          u.handleResponse(res, () => {
            u.success('收藏成功');
            triggerRefresh(true);
          });
        })
        .finally(() => {
          _loading(false);
        });
    }
  };

  return (
    <>
      <Box
        sx={{
          display: 'inline-block',
          position: 'absolute',
          right: 40,
          top: -10,
        }}
      >
        <XLoginCheck>
          <Fab
            sx={{
              zIndex: 1,
            }}
            disabled={loading}
            onClick={() => {
              addCollectOrNot();
            }}
          >
            <FavoriteIcon sx={{ color: model.IsFavorite ? 'red' : 'white' }} />
          </Fab>
        </XLoginCheck>
      </Box>
    </>
  );
};

export default index;
