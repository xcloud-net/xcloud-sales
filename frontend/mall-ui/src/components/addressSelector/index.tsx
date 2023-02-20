import * as React from 'react';
import http from '@/utils/http';
import u from '@/utils';
import XSelect from './select';
import { Grid } from '@mui/material';
import LinearProgress from '@/components/loading/linear';

export default function MultipleSelectChip(props: any) {
  const {
    onChange,
    selected,
    onProvinceChange,
    onCityChange,
    onAreaChange,
  } = props;

  const [loading, _loading] = React.useState(false);

  const [province, _province] = React.useState<any[]>([]);
  const [city, _city] = React.useState<any[]>([]);
  const [area, _area] = React.useState<any[]>([]);

  const [selectedProvince, _selectedProvince] = React.useState<string>('');
  const [selectedCity, _selectedCity] = React.useState<string>('');
  const [selectedArea, _selectedArea] = React.useState<string>('');

  const queryRegion = (parentId: string | null, cb: Function) => {
    _loading(true);
    http.platformRequest
      .post('/region/by-parent', {
        Id: parentId,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          cb && cb(res.data.Data || []);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  const queryProvince = (cb: Function) => {
    queryRegion(null, (province: any) => {
      cb && cb(province);
    });
  };

  const queryCity = (provinceId: string, cb: Function) => {
    queryRegion(provinceId, (city: any) => {
      cb && cb(city);
    });
  };

  const queryArea = (cityId: string, cb: Function) => {
    queryRegion(cityId, (area: any) => {
      cb && cb(area);
    });
  };

  const formatSelectedId = (id: string, data: Array<any>) => {
    if (u.isEmpty(data) || u.isEmpty(id)) {
      return '';
    }

    var selected = u.find(data, (x) => x.Id == id);
    if (!selected) {
      id = u.first(data)?.Id;
    }
    return id;
  };

  const setSelected = (provinceId: string, cityId: string, areaId: string) => {
    var provinceData: Array<any> = [];
    var cityData: Array<any> = [];
    var areaData: Array<any> = [];

    const setData = () => {
      _province(provinceData);
      _city(cityData);
      _area(areaData);
      //
      _selectedProvince(provinceId);
      _selectedCity(cityId);
      _selectedArea(areaId);
    };

    queryProvince((p: Array<any>) => {
      provinceData = p;
      //provinceId = formatSelectedId(provinceId, provinceData);
      setData();
      var selectedProvince = u.find(provinceData, (x) => x.Id == provinceId);
      if (selectedProvince) {
        queryCity(selectedProvince.Id, (c: Array<any>) => {
          cityData = c;
          //cityId = formatSelectedId(cityId, cityData);
          setData();
          var selectedCity = u.find(cityData, (x) => x.Id == cityId);
          if (selectedCity) {
            queryArea(selectedCity.Id, (a: Array<any>) => {
              areaData = a;
              //areaId = formatSelectedId(areaId, areaData);
              setData();
            });
          }
        });
      }
    });

    setData();
  };

  React.useEffect(() => {
    onChange && onChange([selectedProvince, selectedCity, selectedArea]);
  }, [selectedProvince, selectedCity, selectedArea]);

  React.useEffect(() => {
    if (!u.isEmpty(selected) && selected.length == 3) {
      setSelected(selected[0], selected[1], selected[2]);
    } else {
      setSelected('-1', '-1', '-1');
    }
  }, []);

  return (
    <>
      {loading && <LinearProgress />}
      <Grid container spacing={2}>
        <Grid item xs={4}>
          <XSelect
            data={province}
            selected={selectedProvince}
            label={'省'}
            onChange={(e: string) => {
              onProvinceChange && onProvinceChange(e);
              _selectedProvince(e);
              _selectedCity('-1');
              _selectedArea('-1');
              queryCity(e, (cityData: Array<any>) => {
                _city(cityData);
                _area([]);
              });
            }}
          />
        </Grid>
        <Grid item xs={4}>
          <XSelect
            data={city}
            selected={selectedCity}
            label={'市'}
            onChange={(e: string) => {
              onCityChange && onCityChange(e);
              _selectedCity(e);
              _selectedArea('-1');
              queryArea(e, (areaData: Array<any>) => {
                _area(areaData);
              });
            }}
          />
        </Grid>
        <Grid item xs={4}>
          <XSelect
            data={area}
            selected={selectedArea}
            label={'区'}
            onChange={(e: string) => {
              onAreaChange && onAreaChange(e);
              _selectedArea(e);
            }}
          />
        </Grid>
      </Grid>
    </>
  );
}
