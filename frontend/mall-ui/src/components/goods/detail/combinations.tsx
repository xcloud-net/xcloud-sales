import XCombinationPrice from '@/components/goods/price';
import u from '@/utils';
import http from '@/utils/http';
import DoneIcon from '@mui/icons-material/Done';
import {
  Alert,
  Box,
  Card,
  CardActionArea,
  CardContent,
  IconButton,
  Paper,
  Skeleton,
  Typography,
  alpha,
} from '@mui/material';
import * as React from 'react';
import XAddCart from './addCart';
import { GoodsCombinationDto, GoodsDto } from '@/utils/models';

export default function CustomDeleteIconChips(props: {
  model: GoodsDto;
  onSelect?: any;
}) {
  const { model, onSelect } = props;

  const [loading, _loading] = React.useState(false);
  const [combinations, _combinations] = React.useState<GoodsCombinationDto[]>(
    [],
  );
  const [selectedId, _selectedId] = React.useState(0);

  const selectedCombination = u.find(combinations, (x) => x.Id === selectedId);

  const hasStockCombinations: GoodsCombinationDto[] = combinations.filter(
    (x) => x.StockQuantity && x.StockQuantity > 0,
  );

  const queryCombination = (id: number) => {
    _loading(true);
    http.apiRequest
      .post('/mall/goods/query-combination', {
        GoodsId: id,
        Specs: [],
      })
      .then((res) => {
        u.handleResponse(res, () => {
          _combinations(res.data.Data || []);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  const renderCombination = (x: GoodsCombinationDto) => {
    var hasStock = x.StockQuantity && x.StockQuantity > 0;
    var selected = x.Id === selectedId;

    return (
      <Paper
        sx={{
          border: (theme) =>
            selected
              ? `1px solid ${theme.palette.success.main}`
              : `1px solid ${theme.palette.grey[300]}`,
          backgroundColor: (theme) =>
            selected
              ? `${alpha(theme.palette.success.main, 0.05)}`
              : theme.palette.background.paper,
          borderRadius: '10px',
          cursor: 'pointer',
          overflow: 'hidden',
        }}
        onClick={() => {
          if (hasStock) {
            //_selectedId(selected ? 0 : x.Id);
            x.Id && _selectedId(x.Id);
          } else {
            u.error('库存不足');
          }
        }}
      >
        <CardActionArea
          sx={{
            padding: '10px',
            m: 0,
          }}
        >
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'row',
              alignItems: 'center',
              justifyContent: 'space-between',
            }}
          >
            <Box sx={{ width: '100%' }}>
              <Typography variant="h5">{x.Name}</Typography>
              <Typography variant="body2" sx={{}}>
                {x.Description || '--'}
              </Typography>
              {hasStock || (
                <Typography variant="overline" color="primary" display="block">
                  无库存
                </Typography>
              )}
            </Box>
            <Box
              sx={{
                width: '150px',
              }}
            >
              <XCombinationPrice model={x} />
            </Box>
            <Box
              sx={{
                color: 'green',
                width: '50px',
              }}
            >
              {selected && (
                <IconButton
                  size="large"
                  color="inherit"
                  sx={{
                    visibility: 'visible',
                  }}
                >
                  <DoneIcon />
                </IconButton>
              )}
            </Box>
          </Box>
        </CardActionArea>
      </Paper>
    );
  };

  const setDefaultSelection = () => {
    if (!u.isEmpty(hasStockCombinations)) {
      var ids = u.map(hasStockCombinations, (x) => x.Id || 0);
      if (ids.indexOf(selectedId) < 0) {
        _selectedId(ids[0]);
      }
    }
  };

  React.useEffect(() => {
    if (model.Id) {
      queryCombination(model.Id);
    }
  }, [model]);

  React.useEffect(() => {
    setDefaultSelection();
  }, [combinations]);

  React.useEffect(() => {
    onSelect && onSelect(selectedId);
  }, [selectedId]);

  return (
    <Card
      sx={{
        borderRadius: 0,
      }}
    >
      <CardContent>
        <Box
          sx={{
            marginBottom: '10px',
          }}
        >
          <Typography variant="h4">选择组合</Typography>
        </Box>
        <Box sx={{}}>
          {loading && (
            <Box sx={{}}>
              <Skeleton />
              <Skeleton animation="wave" />
              <Skeleton animation={false} />
            </Box>
          )}

          {loading || (
            <>
              {u.isEmpty(combinations) && (
                <Alert severity="info">暂无可售</Alert>
              )}

              {u.map(combinations || [], (x, index) => {
                return (
                  <Box sx={{ mt: 1 }} key={index}>
                    {renderCombination(x)}
                  </Box>
                );
              })}
            </>
          )}
        </Box>
      </CardContent>
      <XAddCart selectedCombination={selectedCombination} />
    </Card>
  );
}
