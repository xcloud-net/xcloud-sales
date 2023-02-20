package com.xcloud.gateway.config;

import cn.hutool.core.util.StrUtil;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.cors.CorsConfiguration;
import org.springframework.web.cors.reactive.CorsWebFilter;
import org.springframework.web.cors.reactive.UrlBasedCorsConfigurationSource;
import org.springframework.web.util.pattern.PathPatternParser;

import java.util.Arrays;
import java.util.stream.Collectors;

@Slf4j
@Configuration
public class CorsConfig {

    @Value("${settings.corsOrigin}")
    private String corsOrigin;

    @Bean
    public CorsWebFilter corsWebFilter() {

        var config = new CorsConfiguration();
        config.addAllowedMethod("*");
        config.addAllowedHeader("*");

        log.info(this.corsOrigin);

        if (StrUtil.isEmpty(this.corsOrigin)) {
            config.addAllowedOrigin("*");
        } else {
            var origins = Arrays
                    .stream(this.corsOrigin.split(","))
                    .map(String::trim)
                    .filter(StrUtil::isNotEmpty)
                    .collect(Collectors.toList());
            origins.forEach(config::addAllowedOrigin);
        }

        UrlBasedCorsConfigurationSource source = new UrlBasedCorsConfigurationSource(new PathPatternParser());
        source.registerCorsConfiguration("/**", config);
        return new CorsWebFilter(source);
    }
}
