package com.xcloud.gateway.controller;

import lombok.extern.slf4j.Slf4j;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.Calendar;

@Slf4j
@RestController
@RequestMapping("api/gateway")
public class HomeController {

    @GetMapping("home")
    public String home() {
        log.info("home");
        return Calendar.getInstance().toString();
    }
}
