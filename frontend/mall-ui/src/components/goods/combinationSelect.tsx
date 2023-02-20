import u from '@/utils';
import { GoodsCombinationDto } from '@/utils/models';
import { Select, Spin } from 'antd';
import { CSSProperties, useCallback, useEffect, useState } from 'react';

export default ({
  selectedCombination,
  onChange,
  style,
}: {
  selectedCombination?: GoodsCombinationDto;
  onChange: any;
  style?: CSSProperties;
}) => {
  const [loading, _loading] = useState(false);
  const [combinations, _combinations] = useState<GoodsCombinationDto[]>([]);
  const [selectedId, _selectedId] = useState<number | undefined>(undefined);

  const queryGoodsImpl = (kwd: string) => {
    if (u.isEmpty(kwd)) {
      _combinations([]);
      return;
    }
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/goods/query-combination-for-selection', {
        Keywords: kwd,
        Take: 20,
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

  const queryGoods = useCallback(u.debounce(queryGoodsImpl, 500), []);

  useEffect(() => {
    if (!selectedId || selectedId == undefined) {
      return;
    }
    var selectedCombination = combinations.find((x) => x.Id == selectedId);
    selectedCombination &&
      onChange &&
      onChange(selectedCombination, (error: string) => {
        console.log(error);
        _selectedId(undefined);
      });
  }, [selectedId]);

  useEffect(() => {
    if (selectedCombination) {
      _combinations([selectedCombination]);
      _selectedId(selectedCombination.Id);
    }
  }, [selectedCombination]);

  return (
    <>
      <Select
        style={{
          minWidth: 300,
          ...(style || {}),
        }}
        loading={loading}
        value={selectedId}
        showSearch
        allowClear
        filterOption={false}
        onSearch={(e) => {
          queryGoods(e);
        }}
        onChange={(e) => {
          _selectedId(e);
        }}
        placeholder="搜索商品规格..."
        notFoundContent={loading && <Spin size="small" />}
      >
        {combinations.map((x, i) => (
          <Select.Option key={i} value={x.Id}>
            <span>{`${x.Goods?.Name || '--'}/${x.Name || '--'}`}</span>
          </Select.Option>
        ))}
      </Select>
    </>
  );
};
