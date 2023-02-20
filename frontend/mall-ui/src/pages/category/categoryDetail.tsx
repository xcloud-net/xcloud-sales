import { Box, Typography, Button } from '@mui/material';
import u from '@/utils';
import { CategoryDto } from '@/utils/models';
import { useEffect, useState } from 'react';
import XLoading from '@/components/loading/dots';

export default (props: {
  model: CategoryDto;
  selectedId: number;
  onSelect: any;
}) => {
  const { model, onSelect, selectedId } = props;
  const [loading, _loading] = useState(false);
  const [data, _data] = useState<CategoryDto[]>([]);

  const triggerSelect = (x: any) => {
    onSelect && onSelect(x);
  };

  const queryChildren = () => {
    if (model && model.Id && model.Id > 0) {
    } else {
      return;
    }
    _loading(true);
    u.http.apiRequest
      .post('/mall/category/by-parent', {
        Id: model.Id,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Data || []);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    _data([]);
    queryChildren();
  }, [model]);

  const renderChildren = () => {
    if (loading) {
      return <XLoading />;
    }
    if (u.isEmpty(data)) {
      return null;
    }
    return (
      <Box sx={{}}>
        {data.map((x, index) => (
          <Button
            sx={{ mr: 1, mb: 1, padding: '2px 5px' }}
            key={index}
            variant={selectedId == x.Id ? 'contained' : 'text'}
            color="primary"
            size="small"
            onClick={() => {
              if (x.Id != selectedId) {
                triggerSelect(x.Id);
              } else {
                triggerSelect(undefined);
              }
            }}
          >
            {x.Name || '--'}
          </Button>
        ))}
      </Box>
    );
  };

  return (
    <>
      <Box sx={{ mb: 3, p: 1 }}>
        <Typography variant="h4" gutterBottom component={'div'}>
          {model.Name || '--'}
        </Typography>
        {u.isEmpty(model.Description) || (
          <Typography
            variant="overline"
            gutterBottom
            color="text.disabled"
            component={'div'}
          >
            {model.Description}
          </Typography>
        )}
        {renderChildren()}
      </Box>
    </>
  );
};
