import XSearchBox from '@/components/searchBox';
import u from '@/utils';
import { SearchGoodsInputDto } from '@/utils/models';
import { Box, Chip, Skeleton, Stack } from '@mui/material';
import React, { useEffect } from 'react';

export default function CustomizedInputBase(props: any) {
  const { finalQuery, options, onSearch, loadingOption } = props;

  const [query, _query] = React.useState<SearchGoodsInputDto>({});

  useEffect(() => {
    _query(finalQuery);
  }, [finalQuery]);

  const triggerSearch = () => {
    onSearch &&
      onSearch({
        ...query,
        Page: 1,
      });
  };

  const renderSelectedBrand = () => {
    if (!(finalQuery.BrandId && finalQuery.BrandId > 0)) {
      return null;
    }

    var brand = options.Brand;

    return (
      <Chip
        size="small"
        label={brand ? `品牌：${brand.Name}` : `品牌`}
        onDelete={() => {
          var param = {
            ...finalQuery,
            Page: 1,
            BrandId: null,
          };
          onSearch && onSearch(param);
        }}
      />
    );
  };

  const renderSelectedCategory = () => {
    if (!(finalQuery.CategoryId && finalQuery.CategoryId > 0)) {
      return null;
    }

    var category = options.Category;

    return (
      <Chip
        size="small"
        label={category ? `类目：${category.Name}` : `类目`}
        onDelete={() => {
          var param = {
            ...finalQuery,
            Page: 1,
            CategoryId: null,
          };
          onSearch && onSearch(param);
        }}
      />
    );
  };

  const renderSelectedTag = () => {
    if (u.isEmpty(finalQuery.TagId)) {
      return null;
    }

    var tag = options.Tag;

    return (
      <Chip
        size="small"
        label={tag ? `标签：${tag.Name}` : `标签`}
        onDelete={() => {
          var param = {
            ...finalQuery,
            Page: 1,
            TagId: null,
          };
          onSearch && onSearch(param);
        }}
      />
    );
  };

  const renderSelectedPriceRange = () => {
    const { PriceMin, PriceMax } = finalQuery;
    const removePriceRange = () => {
      var param = {
        ...finalQuery,
        Page: 1,
        PriceMin: null,
        PriceMax: null,
      };
      onSearch && onSearch(param);
    };

    if (PriceMin && PriceMax) {
      return (
        <Chip
          size="small"
          label={`价格：${PriceMin} ~ ${PriceMax}`}
          onDelete={() => {
            removePriceRange();
          }}
        />
      );
    }
    if (PriceMin) {
      return (
        <Chip
          size="small"
          label={`价格：> ${PriceMin}`}
          onDelete={() => {
            removePriceRange();
          }}
        />
      );
    }
    if (PriceMax) {
      return (
        <Chip
          size="small"
          label={`价格：< ${PriceMin}`}
          onDelete={() => {
            removePriceRange();
          }}
        />
      );
    }
    return null;
  };

  return (
    <>
      <XSearchBox
        keywords={query.Keywords}
        onChange={(e: string) => {
          _query({
            ...query,
            Keywords: e,
          });
        }}
        onSearch={(e: string) => {
          triggerSearch();
        }}
      />
      <Box
        sx={{
          marginTop: 1,
          marginBottom: 1,
        }}
      >
        {loadingOption && (
          <Box sx={{}}>
            <Skeleton animation="wave" />
          </Box>
        )}
        {loadingOption || (
          <Box
            sx={{
              overflowY: 'hidden',
              overflowX: 'scroll',
              'scrollbar-width': 'none',
              '-ms-overflow-style': 'none',
              '-webkit-overflow-scrolling': 'touch',
            }}
          >
            <Stack spacing={1} direction="row">
              {renderSelectedBrand()}
              {renderSelectedCategory()}
              {renderSelectedTag()}
              {renderSelectedPriceRange()}
            </Stack>
          </Box>
        )}
      </Box>
    </>
  );
}
